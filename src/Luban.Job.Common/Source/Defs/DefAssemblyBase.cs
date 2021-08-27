
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
                case "bool": return tags == null ? (nullable ? TBool.NullableIns : TBool.Ins) : new TBool(nullable) { Tags = tags };
                case "uint8":
                case "byte": return tags == null ? (nullable ? TByte.NullableIns : TByte.Ins) : new TByte(nullable) { Tags = tags };
                case "int16":
                case "short": return tags == null ? (nullable ? TShort.NullableIns : TShort.Ins) : new TShort(nullable) { Tags = tags };
                case "fint16":
                case "fshort": return tags == null ? (nullable ? TFshort.NullableIns : TFshort.Ins) : new TFshort(nullable) { Tags = tags };
                case "int32":
                case "int": return tags == null ? (nullable ? TInt.NullableIns : TInt.Ins) : new TInt(nullable) { Tags = tags };
                case "fint32":
                case "fint": return tags == null ? (nullable ? TFint.NullableIns : TFint.Ins) : new TFint(nullable) { Tags = tags };
                case "int64":
                case "long": return tags == null ? (nullable ? TLong.NullableIns : TLong.Ins) : new TLong(nullable, false) { Tags = tags };
                case "bigint": return tags == null ? (nullable ? TLong.NullableBigIns : TLong.BigIns) : new TLong(nullable, true) { Tags = tags };
                case "fint64":
                case "flong": return tags == null ? (nullable ? TFlong.NullableIns : TFlong.Ins) : new TFlong(nullable) { Tags = tags };
                case "float32":
                case "float": return tags == null ? (nullable ? TFloat.NullableIns : TFloat.Ins) : new TFloat(nullable) { Tags = tags };
                case "float64":
                case "double": return tags == null ? (nullable ? TDouble.NullableIns : TDouble.Ins) : new TDouble(nullable) { Tags = tags };
                case "bytes": return tags == null ? (nullable ? new TBytes(true) : TBytes.Ins) : new TBytes(false) { Tags = tags };
                case "string": return tags == null ? (nullable ? TString.NullableIns : TString.Ins) : new TString(nullable) { Tags = tags };
                case "text": return tags == null ? (nullable ? TText.NullableIns : TText.Ins) : new TText(nullable) { Tags = tags };
                case "vector2": return tags == null ? (nullable ? TVector2.NullableIns : TVector2.Ins) : new TVector2(nullable) { Tags = tags };
                case "vector3": return tags == null ? (nullable ? TVector3.NullableIns : TVector3.Ins) : new TVector3(nullable) { Tags = tags };
                case "vector4": return tags == null ? (nullable ? TVector4.NullableIns : TVector4.Ins) : new TVector4(nullable) { Tags = tags };
                case "datetime": return SupportDatetimeType ? (tags == null ? (nullable ? TDateTime.NullableIns : TDateTime.Ins) : new TDateTime(nullable) { Tags = tags }) : throw new NotSupportedException($"只有配置支持datetime数据类型");
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
                    throw new ArgumentException($"invalid container type. module:'{module}' container:'{containerType}' element:'{elementType}'");
                }
            }
        }
    }
}
