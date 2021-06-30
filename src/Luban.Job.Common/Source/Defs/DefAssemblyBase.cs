
using Luban.Common.Utils;
using Luban.Job.Common.Types;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Luban.Job.Common.Defs
{
    public abstract class DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        public Dictionary<string, DefTypeBase> Types { get; } = new Dictionary<string, DefTypeBase>();

        public RemoteAgent Agent { get; protected set; }

        public string TopModule { get; protected set; }

        public bool SupportDatetimeType { get; protected set; }

        public void AddType(DefTypeBase type)
        {
            string fullName = type.FullName;
            if (Types.ContainsKey(fullName))
            {
                throw new Exception($"type:{fullName} duplicate");
            }
            Types.Add(fullName, type);
        }

        public DefTypeBase GetDefType(string fullName)
        {
            return Types.TryGetValue(fullName, out var type) ? type : null;
        }

        public DefTypeBase GetDefType(string module, string type)
        {
            if (Types.TryGetValue(type, out var t))
            {
                return t;
            }
            else if (Types.TryGetValue(TypeUtil.MakeFullName(module, type), out t))
            {
                return t;
            }
            else
            {
                return null;
            }
        }

        private readonly Dictionary<(DefTypeBase, bool), TType> cacheDefTTypes = new Dictionary<(DefTypeBase, bool), TType>();

        protected TType GetOrCreateTEnum(DefEnum defType, bool nullable)
        {
            if (cacheDefTTypes.TryGetValue((defType, nullable), out var t))
            {
                return t;
            }
            else
            {
                return cacheDefTTypes[(defType, nullable)] = new TEnum(defType, nullable);
            }
        }

        protected TType GetOrCreateTBean(DefTypeBase defType, bool nullable)
        {
            if (cacheDefTTypes.TryGetValue((defType, nullable), out var t))
            {
                return t;
            }
            else
            {
                return cacheDefTTypes[(defType, nullable)] = new TBean((DefBeanBase)defType, nullable);
            }
        }

        public TType GetDefTType(string module, string type, bool nullable)
        {
            var defType = GetDefType(module, type);
            switch (defType)
            {
                case DefBeanBase d: return GetOrCreateTBean(d, nullable);
                case DefEnum d: return GetOrCreateTEnum(d, nullable);
                default: return null;
            }
        }

        public List<T> GetDefTypesByType<T>() where T : DefTypeBase
        {
            return Types.Values.Where(v => typeof(T).IsAssignableFrom(v.GetType())).Select(v => (T)v).ToList();
        }

        public TType CreateType(string module, string type)
        {
            int sepIndex = type.IndexOf(',', System.StringComparison.Ordinal);
            if (sepIndex > 0)
            {
                string containerType = type.Substring(0, sepIndex).Trim();
                return CreateContainerType(module, containerType, type.Substring(sepIndex + 1, type.Length - sepIndex - 1).Trim());
            }
            else
            {
                return CreateNotContainerType(module, type);
            }
        }

        protected TType CreateNotContainerType(string module, string type)
        {
            bool nullable;
            if (type.EndsWith('?'))
            {
                nullable = true;
                type = type[0..^1];
            }
            else
            {
                nullable = false;
            }
            switch (type)
            {
                case "bool": return nullable ? TBool.NullableIns : TBool.Ins;
                case "byte": return nullable ? TByte.NullableIns : TByte.Ins;
                case "short": return nullable ? TShort.NullableIns : TShort.Ins;
                case "fshort": return nullable ? TFshort.NullableIns : TFshort.Ins;
                case "int": return nullable ? TInt.NullableIns : TInt.Ins;
                case "fint": return nullable ? TFint.NullableIns : TFint.Ins;
                case "long": return nullable ? TLong.NullableIns : TLong.Ins;
                case "bigint": return nullable ? TLong.NullableBigIns : TLong.BigIns;
                case "flong": return nullable ? TFlong.NullableIns : TFlong.Ins;
                case "float": return nullable ? TFloat.NullableIns : TFloat.Ins;
                case "double": return nullable ? TDouble.NullableIns : TDouble.Ins;
                case "bytes": return TBytes.Ins;
                case "string": return nullable ? TString.NullableIns : TString.Ins;
                case "text": return nullable ? TText.NullableIns : TText.Ins;
                case "vector2": return nullable ? TVector2.NullableIns : TVector2.Ins;
                case "vector3": return nullable ? TVector3.NullableIns : TVector3.Ins;
                case "vector4": return nullable ? TVector4.NullableIns : TVector4.Ins;
                case "datetime": return SupportDatetimeType ? (nullable ? TDateTime.NullableIns : TDateTime.Ins) : throw new NotSupportedException($"只有配置支持datetime数据类型");
                default:
                {
                    var dtype = GetDefTType(module, type, nullable);
                    if (dtype != null)
                    {
                        return dtype;
                    }
                    else
                    {
                        throw new ArgumentException($"invalid type. module:{module} type:{type}");
                    }
                }
            }
        }

        protected TMap CreateMapType(string module, string keyValueType, bool isTreeMap)
        {
            string[] elementTypes = keyValueType.Split(',').Select(s => s.Trim()).ToArray();
            if (elementTypes.Length != 2)
            {
                throw new ArgumentException($"invalid map element type: {keyValueType}");
            }
            return new TMap(CreateNotContainerType(module, elementTypes[0]), CreateNotContainerType(module, elementTypes[1]), isTreeMap);
        }

        protected TType CreateContainerType(string module, string containerType, string elementType)
        {
            switch (containerType)
            {
                case "array": return new TArray(CreateNotContainerType(module, elementType));
                case "list": return new TList(CreateNotContainerType(module, elementType), false);
                case "linkedlist": return new TList(CreateNotContainerType(module, elementType), true);
                case "arraylist": return new TList(CreateNotContainerType(module, elementType), false);
                case "set": return new TSet(CreateNotContainerType(module, elementType), false);
                case "hashset": return new TSet(CreateNotContainerType(module, elementType), true);
                case "treeset": return new TSet(CreateNotContainerType(module, elementType), false);
                case "map": return CreateMapType(module, elementType, false);
                case "treemap": return CreateMapType(module, elementType, true);
                case "hashmap": return CreateMapType(module, elementType, false);
                default:
                {
                    throw new ArgumentException($"invalid container type. module:{module} container:{containerType} element:{elementType}");
                }
            }
        }
    }
}
