// Copyright 2025 Code Philosophy

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Luban.Tests.Helpers;

public static class CsharpCompileHarness
{
    public static object LoadTables(string codeDir, string dataDir, string codeTarget)
    {
        var id = Guid.NewGuid().ToString("N");
        var workDir = Path.Combine(Path.GetTempPath(), "luban-csharp-harness", id);
        Directory.CreateDirectory(workDir);
        // Keep collectible ALC alive for the returned object lifetime; unique AssemblyName avoids LoadFrom conflicts.
        var alc = new AssemblyLoadContext($"luban-test-{id}", isCollectible: true);
        CopyDirectory(codeDir, Path.Combine(workDir, "Gen"));
        CopyRuntime(workDir);

        var csprojPath = Path.Combine(workDir, "GeneratedConsumer.csproj");
        File.WriteAllText(csprojPath, BuildCsproj(codeTarget, id), Encoding.UTF8);

        var build = RunDotnet(["build", csprojPath, "-c", "Release", "-v", "q"]);
        Assert.True(build.ExitCode == 0, $"dotnet build failed:\n{build.Output}");

        var dll = Path.Combine(workDir, "bin", "Release", "net8.0", $"GeneratedConsumer_{id}.dll");
        Assert.True(File.Exists(dll), $"Built DLL missing: {dll}");

        var asm = alc.LoadFromAssemblyPath(dll);
        var tablesType = asm.GetTypes().FirstOrDefault(t => t.Name == "Tables")
            ?? throw new InvalidOperationException("Generated Tables type not found");

        var ctor = tablesType.GetConstructors().Single();
        var loaderType = ctor.GetParameters().Single().ParameterType;
        var loader = CreateLoader(loaderType, dataDir);
        // Hold ALC on the returned instance via conditional weak table isn't needed for tests;
        // GC roots the tables object which roots the assembly which roots the ALC.
        GC.KeepAlive(alc);
        return ctor.Invoke(new object[] { loader });
    }

    public static T GetProp<T>(object obj, string name)
    {
        var type = obj.GetType();
        var prop = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (prop != null)
        {
            return (T)prop.GetValue(obj);
        }

        var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        Assert.True(field != null, $"Member '{name}' not found on {type.FullName}. Members: "
            + string.Join(", ", type.GetMembers(BindingFlags.Instance | BindingFlags.Public).Select(m => m.Name)));
        return (T)field.GetValue(obj);
    }

    public static object Invoke(object obj, string methodName, params object[] args)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
        Assert.NotNull(method);
        return method.Invoke(obj, args);
    }

    private static Delegate CreateLoader(Type funcType, string dataDir)
    {
        // Func<string, TPayload>
        var payloadType = funcType.GetGenericArguments()[1];
        var nameParam = Expression.Parameter(typeof(string), "name");

        Expression body;
        if (payloadType == typeof(JsonElement))
        {
            var method = typeof(CsharpCompileHarness).GetMethod(nameof(LoadJsonElement), BindingFlags.NonPublic | BindingFlags.Static)!;
            body = Expression.Call(method, Expression.Constant(dataDir), nameParam);
        }
        else if (payloadType.Name == "ByteBuf")
        {
            var method = typeof(CsharpCompileHarness).GetMethod(nameof(LoadByteBuf), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(payloadType);
            body = Expression.Call(method, Expression.Constant(dataDir), nameParam);
        }
        else if (payloadType.Name == "JSONNode" || (payloadType.Namespace?.Contains("SimpleJSON") ?? false))
        {
            var method = typeof(CsharpCompileHarness).GetMethod(nameof(LoadJsonNode), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(payloadType);
            body = Expression.Call(method, Expression.Constant(dataDir), Expression.Constant(payloadType.Assembly), nameParam);
        }
        else if (payloadType.FullName == "Newtonsoft.Json.Linq.JArray")
        {
            var method = typeof(CsharpCompileHarness).GetMethod(nameof(LoadJArray), BindingFlags.NonPublic | BindingFlags.Static)!;
            body = Expression.Call(method, Expression.Constant(dataDir), nameParam);
        }
        else
        {
            throw new NotSupportedException($"Unsupported Tables loader payload type: {payloadType.FullName}");
        }

        return Expression.Lambda(funcType, body, nameParam).Compile();
    }

    private static JsonElement LoadJsonElement(string dataDir, string name)
    {
        var path = ResolveDataFile(dataDir, name, ".json");
        using var doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
        return doc.RootElement.Clone();
    }

    private static TByteBuf LoadByteBuf<TByteBuf>(string dataDir, string name)
    {
        var path = ResolveDataFile(dataDir, name, ".bytes");
        return (TByteBuf)Activator.CreateInstance(typeof(TByteBuf), File.ReadAllBytes(path))!;
    }

    private static TNode LoadJsonNode<TNode>(string dataDir, Assembly asm, string name)
    {
        var path = ResolveDataFile(dataDir, name, ".json");
        var text = File.ReadAllText(path, Encoding.UTF8);
        var jsonType = asm.GetType("Luban.SimpleJSON.JSON")
            ?? throw new InvalidOperationException("Luban.SimpleJSON.JSON not found in generated assembly");
        var parse = jsonType.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) })
            ?? throw new InvalidOperationException("JSON.Parse(string) not found");
        return (TNode)parse.Invoke(null, new object[] { text })!;
    }

    private static Newtonsoft.Json.Linq.JArray LoadJArray(string dataDir, string name)
    {
        var path = ResolveDataFile(dataDir, name, ".json");
        return Newtonsoft.Json.Linq.JArray.Parse(File.ReadAllText(path, Encoding.UTF8));
    }

    private static string ResolveDataFile(string dataDir, string name, string preferredExt)
    {
        var preferred = Path.Combine(dataDir, name + preferredExt);
        if (File.Exists(preferred))
        {
            return preferred;
        }

        var matches = Directory.GetFiles(dataDir, name + ".*", SearchOption.TopDirectoryOnly);
        Assert.True(matches.Length > 0, $"Data file not found for table '{name}' under {dataDir}");
        return matches[0];
    }

    private static string BuildCsproj(string codeTarget, string assemblyId)
    {
        var packages = codeTarget.Contains("newtonsoft", StringComparison.OrdinalIgnoreCase)
            ? """    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />"""
            : "";

        return $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>disable</Nullable>
                <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
                <OutputType>Library</OutputType>
                <AssemblyName>GeneratedConsumer_{assemblyId}</AssemblyName>
              </PropertyGroup>
              <ItemGroup>
            {packages}
              </ItemGroup>
            </Project>
            """;
    }

    private static void CopyRuntime(string workDir)
    {
        var runtimeSrc = TestPaths.RuntimeSupportRoot;
        foreach (var file in Directory.GetFiles(runtimeSrc, "*.cs", SearchOption.AllDirectories))
        {
            if (file.Contains("Unity", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var relative = Path.GetRelativePath(runtimeSrc, file);
            var dest = Path.Combine(workDir, "Runtime", relative);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, overwrite: true);
        }
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var dest = Path.Combine(destination, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            File.Copy(file, dest, overwrite: true);
        }
    }

    private static (int ExitCode, string Output) RunDotnet(string[] args)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (var arg in args)
        {
            psi.ArgumentList.Add(arg);
        }

        using var proc = Process.Start(psi)!;
        var stdout = proc.StandardOutput.ReadToEnd();
        var stderr = proc.StandardError.ReadToEnd();
        proc.WaitForExit();
        return (proc.ExitCode, stdout + stderr);
    }
}
