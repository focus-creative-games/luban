using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cpp_ue_bp")]
    class CppUE4BpRender : CodeRenderBase
    {

        public override void Render(GenContext ctx)
        {
            foreach (var c in ctx.ExportTypes)
            {
                if (!(c is DefEnum || c is DefBean))
                {
                    continue;
                }

                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(RenderAny(c), ELanguage.CPP);
                    var file = "bp_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }

        [ThreadStatic]
        private static Template t_enumRender;
        public override string Render(DefEnum e)
        {
            // ue 不允许 UEnum 为这
            // ue 强制枚举underling type 为 uint8, 意味着不能超过255
            var template = t_enumRender ??= Template.Parse(@"
#pragma once
#include ""CoreMinimal.h""

#include ""{{ue_bp_header_file_name_without_suffix}}.generated.h""

UENUM(BlueprintType)
enum class {{ue_bp_full_name}} : uint8
{
    {{~if !contains_value_equal0_item~}}
    __DEFAULT__ = 0,
    {{~end~}}
    {{~if contains_any_ue_enum_compatible_item~}}
    {{~for item in items ~}}
    {{if item.int_value >= 256}}//{{end}}{{item.name}} = {{item.value}}     UMETA(DisplayName = ""{{item.alias_or_name}}""),
    {{~end~}}
    {{~else~}}
    DUMMY UMETA(DisplayName = ""DUMMY""),
    {{~end~}}
};

");
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public override string Render(DefBean b)
        {
            var template = t_beanRender ??= Template.Parse(@"
#pragma once
#include ""CoreMinimal.h""
#include ""UCfgObj.h""


{{ue_bp_includes}}

#include ""{{ue_bp_header_file_name_without_suffix}}.generated.h""

UCLASS(BlueprintType)
class X6PROTO_API {{ue_bp_full_name}} : public {{if parent_def_type}} {{parent_def_type.ue_bp_full_name}} {{else}} UCfgObj {{end}}
{
	GENERATED_BODY()

public:


    {{~for field in export_fields ~}}
	UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (DisplayName = ""{{field.name}}""))
    {{field.ctype.ue_bp_cpp_define_type}} {{field.name}};
    {{~end~}}
};


");
            var result = template.Render(b);

            return result;
        }

        public override string Render(DefTable c)
        {
            throw new NotImplementedException();
        }

        public override string RenderService(string name, string module, List<DefTable> tables)
        {
            throw new NotImplementedException();
        }
    }
}
