using Luban.Job.Common.Types;
using System;

namespace Luban.Job.Cfg.TypeVisitors
{
    class EditorUeCppDefineTypeVisitor : UeBpCppDefineTypeVisitor
    {
        public static new EditorUeCppDefineTypeVisitor Ins { get; } = new EditorUeCppDefineTypeVisitor();


        public override string Accept(TEnum type)
        {
            //return type.DefineEnum.UeFfullName;
            throw new NotImplementedException();
        }


        public override string Accept(TBean type)
        {
            //return $"TSharedPtr<{type.Bean.UeFfullName}>";
            throw new NotImplementedException();
        }

    }
}
