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
using Luban.CustomBehaviour;
using Luban.Utils;

namespace Luban.Schema;

public class SchemaManager
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static SchemaManager Ins { get; } = new();

    private class LoaderInfo
    {
        public string Type { get; init; }

        public string ExtName { get; init; }

        public int Priority { get; init; }

        public Func<ISchemaLoader> Creator { get; init; }
    }

    private readonly Dictionary<(string, string), LoaderInfo> _schemaLoaders = new();

    public void Init()
    {
        _schemaLoaders.Clear();
    }

    public void ScanRegisterAll(Assembly assembly)
    {
        ScanRegisterSchemaLoaderCreator(assembly);
    }

    public ISchemaCollector CreateSchemaCollector(string name)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<ISchemaCollector, SchemaCollectorAttribute>(name);
    }

    public ISchemaLoader CreateSchemaLoader(string extName, string type, ISchemaCollector collector)
    {
        if (!_schemaLoaders.TryGetValue((extName, type), out var loader))
        {
            throw new Exception($"can't find schema loader for type:{type} extName:{extName}");
        }

        ISchemaLoader schemaLoader = loader.Creator();
        schemaLoader.Type = type;
        schemaLoader.Collector = collector;
        return schemaLoader;
    }

    public void RegisterSchemaLoaderCreator(string type, string extName, int priority, Func<ISchemaLoader> creator)
    {
        if (_schemaLoaders.TryGetValue((extName, type), out var loader))
        {
            if (loader.Priority >= priority)
            {
                s_logger.Warn("schema loader creator already exist. type:{} priority:{} extName:{}", type, priority, extName);
                return;
            }
        }
        s_logger.Trace("add schema loader creator. type:{} priority:{} extName:{}", type, priority, extName);
        _schemaLoaders[(extName, type)] = new LoaderInfo() { Type = type, ExtName = extName, Priority = priority, Creator = creator };
    }

    public void ScanRegisterSchemaLoaderCreator(Assembly assembly)
    {
        foreach (var t in assembly.GetTypes())
        {
            if (t.IsDefined(typeof(SchemaLoaderAttribute), false))
            {
                foreach (var attr in t.GetCustomAttributes<SchemaLoaderAttribute>())
                {
                    var creator = () => (ISchemaLoader)Activator.CreateInstance(t);
                    foreach (var extName in attr.ExtNames)
                    {
                        RegisterSchemaLoaderCreator(attr.Type, extName, attr.Priority, creator);
                    }
                }
            }
        }
    }

    public IBeanSchemaLoader CreateBeanSchemaLoader(string type)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<IBeanSchemaLoader, BeanSchemaLoaderAttribute>(type);
    }

    public ITableImporter CreateTableImporter(string type)
    {
        return CustomBehaviourManager.Ins.CreateBehaviour<ITableImporter, TableImporterAttribute>(type);
    }
}
