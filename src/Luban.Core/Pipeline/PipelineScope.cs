// Copyright 2025 Code Philosophy
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.CustomBehaviour;
using Luban.DataLoader;
using Luban.DataTarget;
using Luban.L10N;
using Luban.OutputSaver;
using Luban.PostProcess;
using Luban.Schema;
using Luban.Tmpl;
using Luban.Validator;
using System.Reflection;

namespace Luban.Pipeline;

/// <summary>
/// Ambient session for one pipeline run. <see cref="Current"/> is stored in
/// <see cref="AsyncLocal{T}"/> and therefore flows through <c>await</c> and <c>Task.Run</c>.
/// Hosts must <see cref="Enter"/> (or <c>using</c> the returned scope) before accessing
/// <c>XxxManager.Ins</c> / <c>EnvManager.Current</c> / <c>GenerationContext.Current</c>.
/// </summary>
public sealed class PipelineScope : IDisposable
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private static readonly AsyncLocal<PipelineScope> s_current = new();

    // Per async-flow so concurrent code targets under the same scope do not overwrite each other.
    private readonly AsyncLocal<ICodeTarget> _currentCodeTarget = new();

    private PipelineScope _parent;
    private int _enterCount;

    public static PipelineScope Current =>
        s_current.Value ?? throw new InvalidOperationException(
            "No active PipelineScope. Wrap the run with PipelineScope.Create() and Enter().");

    public static bool HasCurrent => s_current.Value != null;

    public EnvManager Env { get; }

    public LubanConfig Config { get; set; }

    public IPipeline Pipeline { get; set; }

    public GenerationContext GenerationContext { get; set; }

    /// <summary>
    /// Current code target for this async flow. Stored in <see cref="AsyncLocal{T}"/> so
    /// multiple code targets can run concurrently within the same <see cref="PipelineScope"/>.
    /// </summary>
    public ICodeTarget CurrentCodeTarget
    {
        get => _currentCodeTarget.Value;
        set => _currentCodeTarget.Value = value;
    }

    public CustomBehaviourManager CustomBehaviour { get; }

    public TemplateManager Templates { get; }

    public SchemaManager Schema { get; }

    public CodeFormatManager CodeFormat { get; }

    public CodeTargetManager CodeTargets { get; }

    public DataTargetManager DataTargets { get; }

    public DataLoaderManager DataLoaders { get; }

    public OutputSaverManager OutputSavers { get; }

    public PostProcessManager PostProcesses { get; }

    public PipelineManager Pipelines { get; }

    public L10NManager L10N { get; }

    public ValidatorManager Validators { get; }

    private PipelineScope(Dictionary<string, string> options)
    {
        Env = new EnvManager(options ?? new Dictionary<string, string>());
        CustomBehaviour = new CustomBehaviourManager();
        Templates = new TemplateManager();
        Schema = new SchemaManager();
        CodeFormat = new CodeFormatManager();
        CodeTargets = new CodeTargetManager();
        DataTargets = new DataTargetManager();
        DataLoaders = new DataLoaderManager();
        OutputSavers = new OutputSaverManager();
        PostProcesses = new PostProcessManager();
        Pipelines = new PipelineManager();
        L10N = new L10NManager();
        Validators = new ValidatorManager();
    }

    /// <summary>
    /// Create a scope, initialize managers, and scan plugin assemblies (same as the old SimpleLauncher.Start).
    /// The returned scope is not entered; call <see cref="Enter"/> before running a pipeline.
    /// </summary>
    public static PipelineScope Create(Dictionary<string, string> options)
    {
        return Create(options, scanPluginAssemblies: true);
    }

    /// <param name="scanPluginAssemblies">
    /// When true, load extra Luban*.dll from the app directory before scanning.
    /// Already-loaded assemblies marked with <see cref="RegisterBehaviourAttribute"/> are always scanned.
    /// </param>
    public static PipelineScope Create(Dictionary<string, string> options, bool scanPluginAssemblies)
    {
        var scope = new PipelineScope(options);
        using (scope.Enter())
        {
            scope.Initialize(scanPluginAssemblies);
        }
        return scope;
    }

    /// <summary>
    /// Push this scope as <see cref="Current"/>. Nested enters of the same instance are counted.
    /// Dispose (or dispose the returned token, which is this instance) to pop.
    /// </summary>
    public IDisposable Enter()
    {
        if (s_current.Value != this)
        {
            _parent = s_current.Value;
            s_current.Value = this;
        }
        _enterCount++;
        return this;
    }

    public void Dispose()
    {
        if (_enterCount <= 0)
        {
            return;
        }
        _enterCount--;
        if (_enterCount == 0 && s_current.Value == this)
        {
            s_current.Value = _parent;
            _parent = null;
        }
    }

    public void Run(Action action)
    {
        using (Enter())
        {
            action();
        }
    }

    public T Run<T>(Func<T> func)
    {
        using (Enter())
        {
            return func();
        }
    }

    public void ScanRegisterAssembly(Assembly assembly)
    {
        CustomBehaviour.ScanRegisterBehaviour(assembly);
        Schema.ScanRegisterAll(assembly);
    }

    private void Initialize(bool scanPluginAssemblies)
    {
        Schema.Init();
        Templates.Init();
        CodeFormat.Init();
        CodeTargets.Init();
        PostProcesses.Init();
        OutputSavers.Init();
        DataLoaders.Init();
        Validators.Init();
        DataTargets.Init();
        Pipelines.Init();
        L10N.Init();
        CustomBehaviour.Init();

        if (scanPluginAssemblies)
        {
            LoadPluginAssemblies();
        }
        ScanLoadedBehaviourAssemblies();

        CodeFormat.PostInit();
    }

    private void LoadPluginAssemblies()
    {
        string dllDir = Path.GetDirectoryName(AppContext.BaseDirectory);
        foreach (var dllFile in Directory.GetFiles(dllDir, "*.dll", SearchOption.TopDirectoryOnly))
        {
            string dllName = Path.GetFileNameWithoutExtension(dllFile);
            if (dllName.Contains("Luban") && AppDomain.CurrentDomain.GetAssemblies().All(a => a.GetName().Name != dllName))
            {
                s_logger.Trace("load dll:{dll}", dllFile);
                Assembly.Load(dllName);
            }
        }
    }

    private void ScanLoadedBehaviourAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetCustomAttribute<RegisterBehaviourAttribute>() != null)
            {
                ScanRegisterAssembly(assembly);
            }
        }
    }
}
