using Luban.Common.Utils;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;

namespace Luban.Job.Cfg.Defs
{
    public abstract class CfgDefTypeBase : DefTypeBase
    {
        public DefAssembly Assembly => (DefAssembly)AssemblyBase;

        public virtual string UeBpName => "U" + Name;

        public virtual string UeBpFullName => TypeUtil.MakeCppJoinedFullName("U" + Namespace, Name);

        public string UeBpHeaderFileName => "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(FullName);

        public string UeBpHeaderFileNameWithoutSuffix => "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePathWithoutSuffix(FullName);

        public string EditorUeFullName => TypeUtil.MakeCppFullName(Namespace, Name);

        public string UeFname => "F" + Name;

        public string UeFfullName => TypeUtil.MakeCppFullName(Namespace, UeFname);

        public string UeHeaderFileName => RenderFileUtil.GetUeCppDefTypeHeaderFilePath(FullName);

        public string UeEditorHeaderFileName => "editor_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(FullName);

    }
}
