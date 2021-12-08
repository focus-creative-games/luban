using Luban.Job.Common.Generate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_protobuf3")]
    class Protobuf3SchemaRender : ProtobufSchemaRenderBase
    {
        protected override string CommonRenderTemplateDir => "protobuf3";

        protected override string RenderTemplateDir => "protobuf3";
    }
}
