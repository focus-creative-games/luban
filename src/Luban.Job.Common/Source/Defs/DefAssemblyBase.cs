
using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
using Luban.Job.Common.TypeVisitors;
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

        public static bool IsUseUnityVectors => LocalAssebmly?.CsUseUnityVectors == true;

        public Dictionary<string, DefTypeBase> Types { get; } = new Dictionary<string, DefTypeBase>();

        public List<DefTypeBase> TypeList { get; } = new List<DefTypeBase>();

        private readonly Dictionary<string, DefTypeBase> _notCaseSenseTypes = new();

        private readonly HashSet<string> _namespaces = new();

        private readonly Dictionary<string, DefTypeBase> _notCaseSenseNamespaces = new();

        public IAgent Agent { get; protected set; }

        public string TopModule { get; protected set; }

        public bool SupportDatetimeType { get; protected set; } = false;

        public bool SupportNullable { get; protected set; } = true;

        public bool CsUseUnityVectors { get; set; }

        public GenArgsBase Args { get; private set; }

        public NamingConvention NamingConventionModule { get; set; } = NamingConvention.LanguangeRecommend;

        public NamingConvention NamingConventionType { get; set; } = NamingConvention.LanguangeRecommend;

        public NamingConvention NamingConventionBeanMember { get; set; } = NamingConvention.LanguangeRecommend;

        public NamingConvention NamingConventionEnumMember { get; set; } = NamingConvention.LanguangeRecommend;

        public AccessConvention AccessConventionBeanMember { get; set; } = AccessConvention.LanguangeRecommend;

        public ELanguage CurrentLanguage { get; set; } = ELanguage.INVALID;

        public HashSet<string> ExternalSelectors { get; private set; }

        private Dictionary<string, ExternalType> ExternalTypes { get; set; }

        private readonly Dictionary<string, ExternalType> _externalTypesByTypeName = new();

        public List<string> CurrentExternalSelectors { get; private set; }

        public Dictionary<string, string> Options { get; private set; }

        public string EditorTopModule { get; private set; }

        public bool ContainsOption(string optionName)
        {
            return Options.ContainsKey(optionName);
        }

        public string GetOption(string optionName)
        {
            return Options.TryGetValue(optionName, out var value) ? value : null;
        }

        public string GetOptionOr(string optionName, string defaultValue)
        {
            return Options.TryGetValue(optionName, out var value) ? value : defaultValue;
        }

        private void SetCurrentExternalSelectors(string selectors)
        {
            if (string.IsNullOrEmpty(selectors))
            {
                CurrentExternalSelectors = new List<string>();
            }
            else
            {

                CurrentExternalSelectors = selectors.Split(',').Select(s => s.Trim()).ToList();
                foreach (var selector in CurrentExternalSelectors)
                {
                    if (!ExternalSelectors.Contains(selector))
                    {
                        throw new Exception($"未知 externalselector:{selector}, 有效值应该为 '{Bright.Common.StringUtil.CollectionToString(ExternalSelectors)}'");
                    }
                }
            }
        }

        public void LoadCommon(DefinesCommon defines, IAgent agent, GenArgsBase args)
        {
            LocalAssebmly = this;

            this.Agent = agent;
            this.TopModule = defines.TopModule;
            this.ExternalSelectors = defines.ExternalSelectors;
            this.ExternalTypes = defines.ExternalTypes;
            this.Options = defines.Options;
            this.EditorTopModule = GetOptionOr("editor.topmodule", TypeUtil.MakeFullName("editor", defines.TopModule));

            this.Args = args;

            SetCurrentExternalSelectors(args.ExternalSelectors);

            CsUseUnityVectors = args.CsUseUnityVectors;
            NamingConventionModule = args.NamingConventionModule;
            NamingConventionType = args.NamingConventionType;
            NamingConventionBeanMember = args.NamingConventionBeanMember;
            NamingConventionEnumMember = args.NamingConventionEnumMember;
        }

        public ExternalTypeMapper GetExternalTypeMapper(TType type)
        {
            return GetExternalTypeMapper(type.Apply(RawDefineTypeNameVisitor.Ins));
        }

        public ExternalTypeMapper GetExternalTypeMapper(string typeName)
        {
            ExternalType externalType = _externalTypesByTypeName.GetValueOrDefault(typeName);
            if (externalType == null)
            {
                return null;
            }
            return externalType.Mappers.Find(m => m.Lan == CurrentLanguage && CurrentExternalSelectors.Contains(m.Selector));
        }

        public ExternalType GetExternalType(string typeName)
        {
            return _externalTypesByTypeName.GetValueOrDefault(typeName);
        }

        private static readonly HashSet<string> s_internalOriginTypes = new HashSet<string>
        {
            "vector2",
            "vector3",
            "vector4",
            "datetime",
        };

        public void AddExternalType(ExternalType type)
        {
            string originTypeName = type.OriginTypeName;
            if (!Types.ContainsKey(originTypeName) && !s_internalOriginTypes.Contains(originTypeName))
            {
                throw new LoadDefException($"externaltype:'{type.Name}' originTypeName:'{originTypeName}' 不存在");
            }
            if (!_externalTypesByTypeName.TryAdd(originTypeName, type))
            {
                throw new LoadDefException($"type:'{originTypeName} 被重复映射. externaltype1:'{type.Name}' exteraltype2:'{_externalTypesByTypeName[originTypeName].Name}'");
            }
        }

        public void AddType(DefTypeBase type)
        {
            string fullName = type.FullName;
            if (Types.ContainsKey(fullName))
            {
                throw new Exception($"type:'{fullName}' duplicate");
            }

            if (!_notCaseSenseTypes.TryAdd(fullName.ToLower(), type))
            {
                throw new Exception($"type:'{fullName}' 和 type:'{_notCaseSenseTypes[fullName.ToLower()].FullName}' 类名小写重复. 在win平台有问题");
            }

            string namespaze = type.Namespace;
            if (_namespaces.Add(namespaze) && !_notCaseSenseNamespaces.TryAdd(namespaze.ToLower(), type))
            {
                throw new Exception($"type:'{fullName}' 和 type:'{_notCaseSenseNamespaces[namespaze.ToLower()].FullName}' 命名空间小写重复. 在win平台有问题，请修改定义并删除生成的代码目录后再重新生成");
            }

            Types.Add(fullName, type);
            TypeList.Add(type);
        }

        public DefTypeBase GetDefType(string fullName)
        {
            return Types.TryGetValue(fullName, out var type) ? type : null;
        }

        public DefTypeBase GetDefType(string module, string type)
        {
            if (Types.TryGetValue(TypeUtil.MakeFullName(module, type), out var t))
            {
                return t;
            }
            else if (Types.TryGetValue(type, out t))
            {
                return t;
            }
            else
            {
                return null;
            }
        }

        private readonly Dictionary<(DefTypeBase, bool), TType> _cacheDefTTypes = new Dictionary<(DefTypeBase, bool), TType>();

        protected TType GetOrCreateTEnum(DefEnum defType, bool nullable, Dictionary<string, string> tags)
        {
            if (tags == null || tags.Count == 0)
            {
                if (_cacheDefTTypes.TryGetValue((defType, nullable), out var t))
                {
                    return t;
                }
                else
                {
                    return _cacheDefTTypes[(defType, nullable)] = TEnum.Create(nullable, defType, tags);
                }
            }
            else
            {
                return TEnum.Create(nullable, defType, tags); ;
            }
        }

        protected TType GetOrCreateTBean(DefTypeBase defType, bool nullable, Dictionary<string, string> tags)
        {
            if (tags == null || tags.Count == 0)
            {
                if (_cacheDefTTypes.TryGetValue((defType, nullable), out var t))
                {
                    return t;
                }
                else
                {
                    return _cacheDefTTypes[(defType, nullable)] = TBean.Create(nullable, (DefBeanBase)defType, tags);
                }
            }
            else
            {
                return TBean.Create(nullable, (DefBeanBase)defType, tags);
            }
        }

        public TType GetDefTType(string module, string type, bool nullable, Dictionary<string, string> tags)
        {
            var defType = GetDefType(module, type);
            switch (defType)
            {
                case DefBeanBase d: return GetOrCreateTBean(d, nullable, tags);
                case DefEnum d: return GetOrCreateTEnum(d, nullable, tags);
                default: return null;
            }
        }

        public List<T> GetDefTypesByType<T>() where T : DefTypeBase
        {
            return Types.Values.Where(v => typeof(T).IsAssignableFrom(v.GetType())).Select(v => (T)v).ToList();
        }

        public TType CreateType(string module, string type, bool containerElementType)
        {
            type = DefUtil.TrimBracePairs(type);
            int sepIndex = DefUtil.IndexOfBaseTypeEnd(type);
            if (sepIndex > 0)
            {
                string containerTypeAndTags = DefUtil.TrimBracePairs(type.Substring(0, sepIndex));
                var elementTypeAndTags = type.Substring(sepIndex + 1);
                var (containerType, containerTags) = DefUtil.ParseTypeAndVaildAttrs(containerTypeAndTags);
                return CreateContainerType(module, containerType, containerTags, elementTypeAndTags.Trim());
            }
            else
            {
                return CreateNotContainerType(module, type, containerElementType);
            }
        }

        protected TType CreateNotContainerType(string module, string rawType, bool containerElementType)
        {
            bool nullable;
            // 去掉 rawType 两侧的匹配的 ()
            rawType = DefUtil.TrimBracePairs(rawType);
            var (type, tags) = DefUtil.ParseTypeAndVaildAttrs(rawType);

            if (type.EndsWith('?'))
            {
                if (!SupportNullable)
                {
                    throw new Exception($"not support nullable type:'{module}.{type}'");
                }
                if (containerElementType)
                {
                    throw new Exception($"container element type can't be nullable type:'{module}.{type}'");
                }
                nullable = true;
                type = type.Substring(0, type.Length - 1);
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
                case "time":
                case "datetime": return SupportDatetimeType ? TDateTime.Create(nullable, tags) : throw new NotSupportedException($"只有配置支持datetime数据类型");
                default:
                    {
                        var dtype = GetDefTType(module, type, nullable, tags);
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

        protected TMap CreateMapType(string module, Dictionary<string, string> tags, string keyValueType, bool isTreeMap)
        {
            int typeSepIndex = DefUtil.IndexOfElementTypeSep(keyValueType);
            if (typeSepIndex <= 0 || typeSepIndex >= keyValueType.Length - 1)
            {
                throw new ArgumentException($"invalid map element type:'{keyValueType}'");
            }
            return TMap.Create(false, tags,
                CreateNotContainerType(module, keyValueType.Substring(0, typeSepIndex).Trim(), true),
                CreateType(module, keyValueType.Substring(typeSepIndex + 1).Trim(), true), isTreeMap);
        }

        protected TType CreateContainerType(string module, string containerType, Dictionary<string, string> containerTags, string elementType)
        {
            switch (containerType)
            {
                case "array":
                    {
                        return TArray.Create(false, containerTags, CreateType(module, elementType, true));
                    }
                case "list": return TList.Create(false, containerTags, CreateType(module, elementType, true), true);
                case "set":
                    {
                        TType type = CreateType(module, elementType, true);
                        if (type.IsCollection)
                        {
                            throw new Exception("set的元素不支持容器类型");
                        }
                        return TSet.Create(false, containerTags, type, false);
                    }
                case "map": return CreateMapType(module, containerTags, elementType, false);
                default:
                    {
                        throw new ArgumentException($"invalid container type. module:'{module}' container:'{containerType}' element:'{elementType}'");
                    }
            }
        }
    }
}
