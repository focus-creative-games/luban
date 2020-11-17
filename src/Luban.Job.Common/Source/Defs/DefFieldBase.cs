using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Defs
{
    public abstract class DefFieldBase
    {
        public DefAssemblyBase AssemblyBase => HostType.AssemblyBase;

        public DefTypeBase HostType { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        private string _lanStyleName;
        public string CsStyleName
        {
            get
            {
                if (_lanStyleName == null)
                {
                    _lanStyleName = TypeUtil.ToCsStyleName(Name);
                }
                return _lanStyleName;
            }
        }

        public string JavaStyleName
        {
            get
            {
                if (_lanStyleName == null)
                {
                    _lanStyleName = TypeUtil.ToJavaStyleName(Name);
                }
                return _lanStyleName;
            }
        }

        public string CppStyleName => JavaStyleName;

        public string TsStyleName
        {
            get
            {
                if (_lanStyleName == null)
                {
                    _lanStyleName = TypeUtil.ToJavaStyleName(Name);
                }
                return _lanStyleName;
            }
        }

        public string PyStyleName => Name;

        public string Type { get; }

        public TType CType { get; protected set; }

        public bool IsNullable => CType.IsNullable;

        public string UpperCaseName => Name.ToUpper();

        public DefFieldBase(DefTypeBase host, Field f, int idOffset)
        {
            HostType = host;
            Id = f.Id + idOffset;
            Name = f.Name;
            Type = f.Type;
        }

        public virtual void Compile()
        {

            if (Id < 0 || Id > 256)
            {
                throw new Exception($"结构:{HostType.FullName} 字段:{Name} id:{Id} 超出范围");
            }
            if (!TypeUtil.IsValidName(Name))
            {
                throw new Exception($"filed name:{Name} is reserved");
            }

            if ((CType = AssemblyBase.CreateType(HostType.Namespace, Type)) == null)
            {
                throw new Exception($"type:{HostType.FullName} filed:{Name} type:{Type} is invalid");
            }

            //if (IsNullable && (CType.IsCollection || (CType is TBean)))
            //{
            //    throw new Exception($"type:{HostType.FullName} filed:{Name} type:{Type} is collection or bean. not support nullable");
            //}

            switch (CType)
            {
                case TArray t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type can't be empty bean");
                    }
                    break;
                }
                case TList t:
                {
                    if (t.ElementType is TBean e && !e.IsDynamic && e.Bean.HierarchyFields.Count == 0)
                    {
                        throw new Exception($"container element type can't be empty bean");
                    }
                    break;
                }
            }
        }

        public virtual void PostCompile()
        {
            // 检查 字段类型 与 所引用的表的key是否一致
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
                        throw new Exception($"type:{hostType.FullName} field:{f.Name} id can't be 0");
                    }
                    if (!ids.Add(f.Id))
                    {
                        throw new Exception($"type:{hostType.FullName} field:{f.Name} id:{f.Id} duplicate");
                    }
                }
            }

            var names = new HashSet<string>();
            foreach (var f in fields)
            {
                var fname = f.Name;
                if (fname.Length == 0)
                {
                    throw new Exception($"type:{hostType.FullName} field id:{f.Id} name can't be empty");
                }
                if (!names.Add(fname))
                {
                    throw new Exception($"type:{hostType.FullName} field:{fname} duplicate");
                }
                if (TypeUtil.ToCsStyleName(fname) == hostType.Name)
                {
                    throw new Exception($"type:{hostType.FullName} 字段:{fname} 生成的c#字段名与类型名相同，会引起编译错误");
                }
            }

            foreach (var f in fields)
            {
                f.Compile();
            }
        }
    }
}
