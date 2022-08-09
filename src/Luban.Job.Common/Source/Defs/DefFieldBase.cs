using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Defs
{
    public abstract class DefFieldBase
    {
        public DefAssemblyBase AssemblyBase => HostType.AssemblyBase;

        public DefTypeBase HostType { get; set; }

        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public int AutoId { get; set; }

        public string ConventionName
        {
            get
            {
                string cn;
                ELanguage curLan = AssemblyBase.CurrentLanguage;
                switch (AssemblyBase.NamingConventionBeanMember)
                {
                    case NamingConvention.None: cn = Name; break;
                    case NamingConvention.CameraCase: cn = TypeUtil.ToCamelCase(Name); break;
                    case NamingConvention.PascalCase: cn = TypeUtil.ToPascalCase(Name); break;
                    case NamingConvention.UnderScores: cn = TypeUtil.ToUnderScores(Name); break;
                    case NamingConvention.Invalid: throw new Exception($"invalid NamingConvention");
                    case NamingConvention.LanguangeRecommend:
                        {
                            switch (curLan)
                            {
                                case ELanguage.INVALID: throw new Exception($"not set current language. can't get recommend naming convention name");
                                case ELanguage.CS: cn = TypeUtil.ToPascalCase(Name); break;
                                case ELanguage.JAVA: cn = TypeUtil.ToCamelCase(Name); break;
                                case ELanguage.GO: cn = TypeUtil.ToPascalCase(Name); break;
                                case ELanguage.CPP: cn = TypeUtil.ToCamelCase(Name); break;
                                case ELanguage.LUA: cn = TypeUtil.ToUnderScores(Name); break;
                                case ELanguage.JAVASCRIPT: cn = TypeUtil.ToCamelCase(Name); break;
                                case ELanguage.TYPESCRIPT: cn = TypeUtil.ToCamelCase(Name); break;
                                case ELanguage.PYTHON: cn = TypeUtil.ToUnderScores(Name); break;
                                case ELanguage.GDSCRIPT: cn = TypeUtil.ToUnderScores(Name); break;
                                case ELanguage.RUST: cn = TypeUtil.ToUnderScores(Name); break;
                                case ELanguage.PROTOBUF: cn = Name; break;
                                default: throw new Exception($"unknown language:{curLan}");
                            }
                            break;
                        }
                    default: throw new Exception($"unknown NamingConvention:{AssemblyBase.NamingConventionBeanMember}");
                }
                if (curLan == ELanguage.RUST)
                {
                    if (cn == "type")
                    {
                        cn = "r#type";
                    }
                }
                return cn;
            }
        }

        public string Type { get; }

        public TType CType { get; protected set; }

        public bool IsNullable => CType.IsNullable;

        public string UpperCaseName => Name.ToUpper();

        public string Comment { get; }

        public string EscapeComment => DefUtil.EscapeCommentByCurrentLanguage(Comment);

        public Dictionary<string, string> Tags { get; }

        public bool IgnoreNameValidation { get; set; }

        public bool HasTag(string attrName)
        {
            return Tags != null && Tags.ContainsKey(attrName);
        }

        public string GetTag(string attrName)
        {
            return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
        }

        public DefFieldBase(DefTypeBase host, Field f, int idOffset)
        {
            HostType = host;
            Id = f.Id + idOffset;
            AutoId = Id;
            Name = f.Name;
            Type = f.Type;
            Comment = f.Comment;
            Tags = DefUtil.ParseAttrs(f.Tags);
            IgnoreNameValidation = f.IgnoreNameValidation;
        }

        public virtual void Compile()
        {

            if (Id < 0 || Id > 256)
            {
                throw new Exception($"type:'{HostType.FullName}' field:'{Name}' id:{Id} 超出范围");
            }
            if (!IgnoreNameValidation && !TypeUtil.IsValidName(Name))
            {
                throw new Exception($"type:'{HostType.FullName}' field name:'{Name}' is reserved");
            }

            try
            {
                CType = AssemblyBase.CreateType(HostType.Namespace, Type, false);
            }
            catch (Exception e)
            {
                throw new Exception($"type:'{HostType.FullName}' field:'{Name}' type:'{Type}' is invalid", e);
            }

            //if (IsNullable && (CType.IsCollection || (CType is TBean)))
            //{
            //    throw new Exception($"type:{HostType.FullName} field:{Name} type:{Type} is collection or bean. not support nullable");
            //}

            switch (CType)
            {
                case TArray t:
                    {
                        if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                        {
                            throw new Exception($"container element type:'{e.Bean.FullName}' can't be empty bean");
                        }
                        break;
                    }
                case TList t:
                    {
                        if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                        {
                            throw new Exception($"container element type:'{e.Bean.FullName}' can't be empty bean");
                        }
                        break;
                    }
            }
        }

        public virtual void PostCompile()
        {
            CType.PostCompile(this);
        }

        public static void CompileFields<T>(DefTypeBase hostType, List<T> fields, bool verifyId) where T : DefFieldBase
        {
            if (verifyId)
            {
                var ids = new HashSet<int>();
                foreach (var f in fields)
                {
                    if (f.Id == 0)
                    {
                        throw new Exception($"type:'{hostType.FullName}' field:'{f.Name}' id can't be 0");
                    }
                    if (!ids.Add(f.Id))
                    {
                        throw new Exception($"type:'{hostType.FullName}' field:'{f.Name}' id:{f.Id} duplicate");
                    }
                }
            }

            var names = new HashSet<string>();
            foreach (var f in fields)
            {
                var fname = f.Name;
                if (fname.Length == 0)
                {
                    throw new Exception($"type:'{hostType.FullName}' field id:{f.Id} name can't be empty");
                }
                if (!names.Add(fname))
                {
                    throw new Exception($"type:'{hostType.FullName}' 'field:{fname}' duplicate");
                }
                if (TypeUtil.ToCsStyleName(fname) == hostType.Name)
                {
                    throw new Exception($"type:'{hostType.FullName}' field:'{fname}' 生成的c#字段名与类型名相同，会引起编译错误");
                }
            }

            foreach (var f in fields)
            {
                f.Compile();
            }
        }
    }
}
