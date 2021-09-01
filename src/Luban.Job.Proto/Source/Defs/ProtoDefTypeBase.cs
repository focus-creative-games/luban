using Luban.Common.Utils;
using Luban.Job.Common.Defs;

namespace Luban.Job.Proto.Defs
{
    abstract class ProtoDefTypeBase : DefTypeBase
    {
        public DefAssembly Assembly => (DefAssembly)AssemblyBase;

        public virtual string UeBpName => "U" + Name;

        public virtual string UeBpFullName => TypeUtil.MakeCppJoinedFullName("U" + Namespace, Name);

        //public string UeBpHeaderFileName => "bp_" + FileUtil.GetUeCppDefTypeHeaderFilePath(FullName);

        //public string UeBpHeaderFileNameWithoutSuffix => "bp_" + FileUtil.GetUeCppDefTypeHeaderFilePathWithoutSuffix(FullName);

        //public string EditorUeFullName => TypeUtil.MakeCppFullName(Namespace, Name);

        //public string UeFname => "F" + Name;

        //public string UeFfullName => TypeUtil.MakeCppFullName(Namespace, UeFname);

        //public string UeHeaderFileName => FileUtil.GetUeCppDefTypeHeaderFilePath(FullName);

        //public string UeEditorHeaderFileName => "editor_" + FileUtil.GetUeCppDefTypeHeaderFilePath(FullName);
    }
}
