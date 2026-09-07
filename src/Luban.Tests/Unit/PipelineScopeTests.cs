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

using Luban.CodeTarget;
using Luban.CustomBehaviour;
using Luban.Defs;
using Luban.Pipeline;
using Luban.Tmpl;
using Xunit;

namespace Luban.Tests;

public class PipelineScopeTests
{
    [Fact]
    public void Current_Throws_WhenNoActiveScope()
    {
        Assert.False(PipelineScope.HasCurrent);
        var ex = Assert.Throws<InvalidOperationException>(() => _ = PipelineScope.Current);
        Assert.Contains("No active PipelineScope", ex.Message);
        Assert.Throws<InvalidOperationException>(() => _ = CustomBehaviourManager.Ins);
        Assert.Throws<InvalidOperationException>(() => _ = EnvManager.Current);
        Assert.Throws<InvalidOperationException>(() => _ = TemplateManager.Ins);
        Assert.Throws<InvalidOperationException>(() => _ = GenerationContext.Current);
    }

    [Fact]
    public async Task TaskRun_Inherits_Current()
    {
        using var scope = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "v" }, scanPluginAssemblies: false);
        using (scope.Enter())
        {
            Assert.Same(scope, PipelineScope.Current);
            Assert.Same(scope.CustomBehaviour, CustomBehaviourManager.Ins);
            Assert.Equal("v", EnvManager.Current.GetOptionRaw("k"));

            var captured = await Task.Run(() =>
            {
                var current = PipelineScope.Current;
                var option = EnvManager.Current.GetOptionRaw("k");
                return (current, option);
            });

            Assert.Same(scope, captured.current);
            Assert.Equal("v", captured.option);
        }

        Assert.False(PipelineScope.HasCurrent);
    }

    [Fact]
    public void NestedEnter_Restores_Parent()
    {
        using var outer = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "outer" }, scanPluginAssemblies: false);
        using var inner = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "inner" }, scanPluginAssemblies: false);

        using (outer.Enter())
        {
            Assert.Equal("outer", EnvManager.Current.GetOptionRaw("k"));
            using (inner.Enter())
            {
                Assert.Same(inner, PipelineScope.Current);
                Assert.Equal("inner", EnvManager.Current.GetOptionRaw("k"));
            }
            Assert.Same(outer, PipelineScope.Current);
            Assert.Equal("outer", EnvManager.Current.GetOptionRaw("k"));
        }

        Assert.False(PipelineScope.HasCurrent);
    }

    [Fact]
    public async Task ConcurrentScopes_DoNotLeak()
    {
        using var scopeA = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "a" }, scanPluginAssemblies: false);
        using var scopeB = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "b" }, scanPluginAssemblies: false);

        using (scopeA.Enter())
        {
            _ = new GenerationContext();
        }
        using (scopeB.Enter())
        {
            _ = new GenerationContext();
        }

        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var taskA = Task.Run(async () =>
        {
            using (scopeA.Enter())
            {
                await start.Task;
                await Task.Delay(30);
                Assert.Same(scopeA, PipelineScope.Current);
                Assert.Same(scopeA.CustomBehaviour, CustomBehaviourManager.Ins);
                return (Env: EnvManager.Current.GetOptionRaw("k"), Ctx: GenerationContext.Current);
            }
        });

        var taskB = Task.Run(async () =>
        {
            using (scopeB.Enter())
            {
                await start.Task;
                await Task.Delay(30);
                Assert.Same(scopeB, PipelineScope.Current);
                Assert.Same(scopeB.CustomBehaviour, CustomBehaviourManager.Ins);
                return (Env: EnvManager.Current.GetOptionRaw("k"), Ctx: GenerationContext.Current);
            }
        });

        start.SetResult();
        var fromA = await taskA;
        var fromB = await taskB;

        Assert.Equal("a", fromA.Env);
        Assert.Equal("b", fromB.Env);
        Assert.Same(scopeA.GenerationContext, fromA.Ctx);
        Assert.Same(scopeB.GenerationContext, fromB.Ctx);
        Assert.NotSame(fromA.Ctx, fromB.Ctx);
        Assert.NotSame(scopeA.CustomBehaviour, scopeB.CustomBehaviour);
    }

    [Fact]
    public void Run_AttachesCurrent_OnDetachedThread()
    {
        using var scope = PipelineScope.Create(new Dictionary<string, string> { ["k"] = "run" }, scanPluginAssemblies: false);
        PipelineScope seen = null;
        var thread = new Thread(() =>
        {
            scope.Run(() =>
            {
                seen = PipelineScope.Current;
            });
        });
        thread.Start();
        thread.Join();
        Assert.Same(scope, seen);
    }

    [Fact]
    public async Task ConcurrentCodeTargets_DoNotOverwrite_CurrentCodeTarget()
    {
        using var scope = PipelineScope.Create(new Dictionary<string, string>(), scanPluginAssemblies: false);
        var targetA = new StubCodeTarget("a");
        var targetB = new StubCodeTarget("b");
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        using (scope.Enter())
        {
            var taskA = Task.Run(async () =>
            {
                GenerationContext.CurrentCodeTarget = targetA;
                await start.Task;
                await Task.Delay(30);
                return GenerationContext.CurrentCodeTarget;
            });

            var taskB = Task.Run(async () =>
            {
                GenerationContext.CurrentCodeTarget = targetB;
                await start.Task;
                await Task.Delay(30);
                return GenerationContext.CurrentCodeTarget;
            });

            start.SetResult();
            var fromA = await taskA;
            var fromB = await taskB;

            Assert.Same(targetA, fromA);
            Assert.Same(targetB, fromB);
        }
    }

    private sealed class StubCodeTarget : ICodeTarget
    {
        public StubCodeTarget(string name) => Name = name;

        public string Name { get; }

        public string FileHeader => "";

        public System.Text.Encoding FileEncoding => System.Text.Encoding.UTF8;

        public void ValidateDefinition(GenerationContext ctx) { }

        public void Handle(GenerationContext ctx, OutputFileManifest manifest) { }

        public string GetPathFromFullName(string fullName) => fullName;

        public void GenerateTables(GenerationContext ctx, List<DefTable> tables, CodeWriter writer) { }

        public void GenerateTable(GenerationContext ctx, DefTable table, CodeWriter writer) { }

        public void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer) { }

        public void GenerateEnum(GenerationContext ctx, DefEnum @enum, CodeWriter writer) { }
    }
}
