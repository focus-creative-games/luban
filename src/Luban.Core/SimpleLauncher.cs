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

using System.Reflection;
using Luban.CodeFormat;
using Luban.CodeTarget;
using Luban.CustomBehaviour;
using Luban.DataLoader;
using Luban.DataTarget;
using Luban.L10N;
using Luban.OutputSaver;
using Luban.Pipeline;
using Luban.PostProcess;
using Luban.Schema;
using Luban.Tmpl;
using Luban.Validator;

namespace Luban;

public class SimpleLauncher
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public void Start(Dictionary<string, string> options)
    {
        EnvManager.Current = new EnvManager(options);
        InitManagers();
        ScanRegisterAssemblyBehaviours();
        PostInitManagers();
    }

    private void InitManagers()
    {
        SchemaManager.Ins.Init();
        TemplateManager.Ins.Init();
        CodeFormatManager.Ins.Init();
        CodeTargetManager.Ins.Init();
        PostProcessManager.Ins.Init();
        OutputSaverManager.Ins.Init();
        DataLoaderManager.Ins.Init();
        ValidatorManager.Ins.Init();
        DataTargetManager.Ins.Init();
        PipelineManager.Ins.Init();
        L10NManager.Ins.Init();
        CustomBehaviourManager.Ins.Init();
    }

    private void PostInitManagers()
    {
        CodeFormatManager.Ins.PostInit();
    }

    private void ScanRegisterAssemblyBehaviours()
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

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.GetCustomAttribute<RegisterBehaviourAttribute>() != null)
            {
                ScanRegisterAssembly(assembly);
            }
        }
    }

    public void ScanRegisterAssembly(Assembly assembly)
    {
        CustomBehaviourManager.Ins.ScanRegisterBehaviour(assembly);
        SchemaManager.Ins.ScanRegisterAll(assembly);
    }
}
