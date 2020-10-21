using Luban.Job.Cfg.Defs;
using Luban.Job.Common.Defs;
using Scriban;
using System;

namespace Luban.Job.Cfg.Generate
{
    class UE4BpCppRender
    {
        public string RenderAny(object o)
        {
            switch (o)
            {
                case DefEnum e: return Render(e);
                case DefBean b: return Render(b);
                //case CTable r: return Render(r);
                default: throw new Exception($"unknown render type:{o}");
            }
        }

        [ThreadStatic]
        private static Template t_enumRender;
        public string Render(DefEnum e)
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
    {{if !contains_value_equal0_item}}
    __DEFAULT__ = 0,
    {{end}}
    {{if contains_any_ue_enum_compatible_item}}
    {{- for item in items }}
    {{if item.int_value >= 256}}//{{end}} {{item.name}} = {{item.value}}     UMETA(DisplayName = ""{{item.alias_or_name}}""),
    {{-end}}
    {{else}}
    DUMMY UMETA(DisplayName = ""DUMMY""),
    {{end}}
};

");
            var result = template.Render(e);

            return result;
        }

        [ThreadStatic]
        private static Template t_beanRender;
        public string Render(DefBean b)
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


    {{- for field in export_fields }}
	UPROPERTY(EditAnywhere, BlueprintReadWrite, meta = (DisplayName = ""{{field.name}}""))
    {{field.ctype.ue_bp_cpp_define_type}} {{field.name}};
    {{-end}}
};


");
            var result = template.Render(b);

            return result;
        }


    }
}
