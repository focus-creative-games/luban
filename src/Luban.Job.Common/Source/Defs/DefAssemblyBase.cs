
using Luban.Common.Utils;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Luban.Job.Common.Defs
{
    public abstract class DefAssemblyBase
    {
        private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly AsyncLocal<DefAssemblyBase> _localAssembly = new();

        public static DefAssemblyBase LocalAssebmly
        {
            get
            {
                return _localAssembly.Value;
            }
            set
            {
                _localAssembly.Value = value;
            }
        }

        public static bool IsUseUnityVectors => LocalAssebmly?.UseUnityVectors == true;

        public Dictionary<string, DefTypeBase> Types { get; } = new Dictionary<string, DefTypeBase>();

        public RemoteAgent Agent { get; protected set; }

        public string TopModule { get; protected set; }

        public bool SupportDatetimeType { get; protected set; } = false;

        public bool SupportNullable { get; protected set; } = true;

        public bool UseUnityVectors { get; set; }

        public void AddType(DefTypeBase type)
        {
            string fullName = type.FullName;
            if (Types.ContainsKey(fullName))
            {
                throw new Exception($"type:'{fullName}' duplicate");
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
                return cacheDefTTypes[(defType, nullable)] = TEnum.Create(nullable, defType);
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
                return cacheDefTTypes[(defType, nullable)] = TBean.Create(nullable, (DefBeanBase)defType);
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

        protected TType CreateNotContainerType(string module, string rawType)
        {
            bool nullable;
            var (type, tags) = DefUtil.ParseType(rawType);

            if (type.EndsWith('?'))
            {
                if (!SupportNullable)
                {
                    throw new Exception($"not support nullable type:'{module}.{type}'");
                }
                nullable = true;
                type = type[0..^1];
            }
            else
            {
                nullable = false;
            }
            switch (type)
            {
                case "bool": return TBool.Create(nullable, tags);
                case "uint8":
                case "byte": return TByte.Create(nullable, tags);
                case "int16":
                case "short": return TShort.Create(nullable, tags);
                case "fint16":
                case "fshort": return TFshort.Create(nullable, tags);
                case "int32":
                case "int": return TInt.Create(nullable, tags);
                case "fint32":
                case "fint": return TFint.Create(nullable, tags);
                case "int64":
                case "long": return TLong.Create(nullable, tags, false);
                case "bigint": return TLong.Create(nullable, tags, true);
                case "fint64":
                case "flong": return TFlong.Create(nullable, tags);
                case "float32":
                case "float": return TFloat.Create(nullable, tags);
                case "float64":
                case "double": return TDouble.Create(nullable, tags);
                case "bytes": return TBytes.Create(nullable, tags);
                case "string": return TString.Create(nullable, tags);
                case "text": return TText.Create(nullable, tags);
                case "vector2": return TVector2.Create(nullable, tags);
                case "vector3": return TVector3.Create(nullable, tags);
                case "vector4": return TVector4.Create(nullable, tags);
                case "datetime": return SupportDatetimeType ? TDateTime.Create(nullable, tags) : throw new NotSupportedException($"只有配置支持datetime数据类型");
                default:
                {
                    var dtype = GetDefTType(module, type, nullable);
                    if (dtype != null)
                    {
                        return dtype;
                    }
                    else
                    {
                        throw new ArgumentException($"invalid type. module:'{module}' type:'{type}'");
                    }
                }
            }
        }

        protected TMap CreateMapType(string module, string keyValueType, bool isTreeMap)
        {
            string[] elementTypes = keyValueType.Split(',').Select(s => s.Trim()).ToArray();
            if (elementTypes.Length != 2)
            {
                throw new ArgumentException($"invalid map element type:'{keyValueType}'");
            }
            return TMap.Create(false, null, CreateNotContainerType(module, elementTypes[0]), CreateNotContainerType(module, elementTypes[1]), isTreeMap);
        }

        protected TType CreateContainerType(string module, string containerType, string elementType)
        {
            switch (containerType)
            {
                case "array": return TArray.Create(false, null, CreateNotContainerType(module, elementType));
                case "list": return TList.Create(false, null, CreateNotContainerType(module, elementType), true);
                case "linkedlist": return TList.Create(false, null, CreateNotContainerType(module, elementType), false);
                case "arraylist": return TList.Create(false, null, CreateNotContainerType(module, elementType), true);
                case "set": return TSet.Create(false, null, CreateNotContainerType(module, elementType), false);
                case "hashset": return TSet.Create(false, null, CreateNotContainerType(module, elementType), false);
                case "treeset": return TSet.Create(false, null, CreateNotContainerType(module, elementType), true);
                case "map": return CreateMapType(module, elementType, false);
                case "treemap": return CreateMapType(module, elementType, true);
                case "hashmap": return CreateMapType(module, elementType, false);
                default:
                {
                    throw new ArgumentException($"invalid container type. module:'{module}' container:'{containerType}' element:'{elementType}'");
                }
            }
        }
    }
}
