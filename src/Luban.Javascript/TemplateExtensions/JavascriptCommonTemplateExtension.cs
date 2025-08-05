using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Javascript.TypeVisitors;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Javascript.TemplateExtensions;

public class JavascriptCommonTemplateExtension : ScriptObject
{

    public static string FullName(DefTypeBase bean)
    {
        return TypeUtil.MakeJavascriptFullName(bean.Namespace, bean.Name);
    }
}
