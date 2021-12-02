using Luban.Common.Utils;
using Luban.Job.Common.RawDefs;
using Luban.Job.Common.Utils;
using Luban.Server.Common;
using System;
using System.Collections.Generic;

namespace Luban.Job.Common.Defs
{
    public abstract class DefTypeBase
    {
        public DefAssemblyBase AssemblyBase { get; set; }

        public int Id { get; protected set; }

        public string TopModule => AssemblyBase.TopModule;

        public IAgent Agent => AssemblyBase.Agent;

        public string Name { get; set; }

        public string Namespace { get; set; }

        protected string _externalTypeName;

        public ExternalType ExternalType { get; private set; }

        public string FullName => TypeUtil.MakeFullName(Namespace, Name);

        public string NamespaceWithTopModule => TypeUtil.MakeNamespace(AssemblyBase.TopModule, Namespace);

        public string FullNameWithTopModule => TypeUtil.MakeFullName(AssemblyBase.TopModule, FullName);

        public string JavaFullName => TypeUtil.MakeFullName(Namespace, Name);

        public string GoFullName => TypeUtil.MakeGoFullName(Namespace, Name);

        public string GoPkgName => TypeUtil.MakeGoPkgName(Namespace);

        public string CppNamespaceBegin => TypeUtil.MakeCppNamespaceBegin(Namespace);

        public string CppNamespaceEnd => TypeUtil.MakeCppNamespaceEnd(Namespace);

        public string CppFullNameWithTopModule => TypeUtil.MakeCppFullName(AssemblyBase.TopModule, FullName);

        public string TypescriptNamespaceBegin => TypeUtil.MakeTypescriptNamespaceBegin(Namespace);

        public string TypescriptNamespaceEnd => TypeUtil.MakeTypescriptNamespaceEnd(Namespace);

        public string CppFullName => TypeUtil.MakeCppFullName(Namespace, Name);

        public string PyFullName => TypeUtil.MakePyFullName(Namespace, Name);

        public string RustFullName => TypeUtil.MakeRustFullName(Namespace, Name);

        public string PbFullName => TypeUtil.MakePbFullName(Namespace, Name);

        public string Comment { get; protected set; }

        public string EscapeComment => DefUtil.EscapeCommentByCurrentLanguage(Comment);

        public Dictionary<string, string> Tags { get; protected set; }

        public bool HasTag(string attrName)
        {
            return Tags != null && Tags.ContainsKey(attrName);
        }

        public string GetTag(string attrName)
        {
            return Tags != null && Tags.TryGetValue(attrName, out var value) ? value : null;
        }

        public ExternalTypeMapper CurrentExternalTypeMapper
        {
            get
            {
                if (ExternalType == null)
                {
                    return null;
                }
                return ExternalType.Mappers.Find(m => m.Lan == DefAssemblyBase.LocalAssebmly.CurrentLanguage);
            }
        }

        public virtual void PreCompile() { }

        public virtual void Compile()
        {
            if (!string.IsNullOrEmpty(_externalTypeName))
            {
                if (AssemblyBase.TryGetExternalType(_externalTypeName, out var type))
                {
                    this.ExternalType = type;
                }
                else
                {
                    throw new Exception($"enum:'{FullName}' 对应的 externaltype:{_externalTypeName} 不存在");
                }
            }
        }

        public virtual void PostCompile() { }
    }
}
