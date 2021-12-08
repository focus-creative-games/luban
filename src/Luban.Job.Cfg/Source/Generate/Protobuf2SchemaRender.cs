using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_protobuf")]
    [Render("code_protobuf2")]
    class Protobuf2SchemaRender : ProtobufSchemaRenderBase
    {
        protected override string CommonRenderTemplateDir => "protobuf2";

        protected override string RenderTemplateDir => "protobuf2";
    }
}
