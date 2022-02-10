using Luban.Common.Protos;
using Luban.Job.Cfg.Defs;
using Luban.Job.Common;
using Luban.Job.Common.Defs;
using Luban.Job.Common.Generate;
using Luban.Job.Common.Utils;
using Scriban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luban.Job.Cfg.Generate
{
    [Render("code_cpp_ue_editor_json")]
    class CppUE4EditorJsonRender : CodeRenderBase
    {
        public override void Render(GenContext ctx)
        {
            var render = new CppUE4EditorJsonRender();

            var renderTypes = ctx.Assembly.Types.Values.Where(c => c is DefEnum || c is DefBean).ToList();

            foreach (var c in renderTypes)
            {
                ctx.Tasks.Add(Task.Run(() =>
                {
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(render.RenderAny(c), ELanguage.CPP);
                    var file = "editor_" + RenderFileUtil.GetUeCppDefTypeHeaderFilePath(c.FullName);
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }

            int TYPE_PER_STUB_FILE = 200;

            for (int i = 0, n = (renderTypes.Count + TYPE_PER_STUB_FILE - 1) / TYPE_PER_STUB_FILE; i < n; i++)
            {
                int index = i;
                ctx.Tasks.Add(Task.Run(() =>
                {
                    int startIndex = index * TYPE_PER_STUB_FILE;
                    var content = FileHeaderUtil.ConcatAutoGenerationHeader(
                        render.RenderStub(renderTypes.GetRange(startIndex, Math.Min(TYPE_PER_STUB_FILE, renderTypes.Count - startIndex))),
                        ELanguage.CPP);
                    var file = $"stub_{index}.cpp";
                    var md5 = CacheFileUtil.GenMd5AndAddCache(file, content, true);
                    ctx.GenCodeFilesInOutputCodeDir.Add(new FileInfo() { FilePath = file, MD5 = md5 });
                }));
            }
        }


        [ThreadStatic]
        private static Template t_enumRender;
        public override string Render(DefEnum e)
        {
            var template = t_enumRender ??= Template.Parse(@"
#pragma once
#include ""CoreMinimal.h""

namespace editor
{

{{cpp_namespace_begin}}

    enum class {{ue_fname}}
    {
        {{~for item in items ~}}
        {{item.name}} = {{item.value}},
        {{~end~}}
    };

    bool X6PROTOEDITOR_API {{ue_fname}}ToString({{ue_fname}} value, FString& s);
    bool X6PROTOEDITOR_API {{ue_fname}}FromString(const FString& s, {{ue_fname}}& x);

{{cpp_namespace_end}}

}
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
#include ""FCfgObj.h""

{{editor_cpp_includes}}

namespace editor
{

{{cpp_namespace_begin}}

struct X6PROTOEDITOR_API {{ue_fname}} : public {{if parent_def_type}} {{parent_def_type.ue_fname}}{{else}}FCfgObj{{end}}
{
    {{~for field in fields ~}}
    {{field.ctype.editor_ue_cpp_define_type}} {{field.name}};
    {{~end~}}

{{~if !is_abstract_type~}}
    bool Load(FJsonObject* _json) override;
    bool Save(FJsonObject*& result) override;
{{~end~}}

    static bool Create(FJsonObject* _json, {{ue_fname}}*& result);
};


{{cpp_namespace_end}}

}

");
            var result = template.Render(b);

            return result;
        }


        private class Stub
        {
            public List<DefTypeBase> Types { get; set; }

            public string Includes
            {
                get
                {
                    var includeTypes = new HashSet<DefTypeBase>(Types);

                    foreach (var type in Types)
                    {
                        if (type is DefBean bean)
                        {
                            foreach (DefBean c in bean.HierarchyNotAbstractChildren)
                            {
                                includeTypes.Add(c);
                            }
                        }
                    }
                    //return string.Join('\n', includeTypes.Select(im => $"#include \"{ im.UeEditorHeaderFileName}\""));
                    throw new NotImplementedException();
                }
            }
        }

        [ThreadStatic]
        private static Template t_stubRender;
        public string RenderStub(List<DefTypeBase> types)
        {
            var template = t_stubRender ??= Template.Parse(@"
#include ""JsonUtil.h""

{{includes}}

namespace editor
{

{{~for type in types~}}
{{type.cpp_namespace_begin}}
{{~if type.is_bean~}}
{{~if type.is_abstract_type~}}
    bool {{type.ue_fname}}::Create(FJsonObject* _json, {{type.ue_fname}}*& result)
    {
        FString type;
        if (_json->TryGetStringField(FString(""{{type.json_type_name_key}}""), type))
        {
            {{~for child in type.hierarchy_not_abstract_children~}}
            if (type == ""{{cs_impl_data_type child x}}"")
            {
                result = new {{child.ue_fname}}();
            } else
            {{~end~}}
            {
                result = nullptr;
                return false;
            }
            if (!result->Load(_json))
            {
                delete result;
                return false;
            }
            return true;
        }
        else
        {
            result = nullptr;
            return false;
        }
    }
{{~else~}}
    bool {{type.ue_fname}}::Create(FJsonObject* _json, {{type.ue_fname}}*& result)
    {
        result = new {{type.ue_fname}}();
        if (!result->Load(_json))
        {
            delete result;
            return false;
        }
        return true;
    }


        bool {{type.ue_fname}}::Save(FJsonObject*& result)
        {
            auto _json = new FJsonObject();
            _json->SetStringField(""{{type.json_type_name_key}}"", ""{{type.name}}"");

{{~for field in type.hierarchy_fields~}}
            {{field.editor_ue_cpp_save}}
{{~end~}}
            result = _json;
            return true;
        }

        bool {{type.ue_fname}}::Load(FJsonObject* _json)
        {
{{~for field in type.hierarchy_fields~}}
            {{field.editor_ue_cpp_load}}
{{~end~}}
            return true;
        }
{{~end~}}
{{~else~}}

bool {{type.ue_fname}}ToString({{type.ue_fname}} value, FString& s)
{
    {{~for item in type.items ~}}
    if (value == {{type.ue_fname}}::{{item.name}}) { s = ""{{item.name}}""; return true; }
    {{~end~}}
    return false;
}
bool {{type.ue_fname}}FromString(const FString& s, {{type.ue_fname}}& value)
{
    {{~for item in type.items ~}}
        if (s == ""{{item.name}}"")
        {
            value = {{type.ue_fname}}::{{item.name}};
            return true;
        }
    {{~end~}}
    return false;
}

{{~end~}}
{{type.cpp_namespace_end}}
{{~end~}}
}
");
            var result = template.Render(new Stub
            {
                Types = types,
            });

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
