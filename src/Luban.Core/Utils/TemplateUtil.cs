using Scriban;

namespace Luban.Core.Utils;

public static class TemplateUtil
{
    public static TemplateContext CreateDefaultTemplateContext()
    {
        return new TemplateContext()
        {
            LoopLimit = 0,
            NewLine = "\n",
        };
    }
}