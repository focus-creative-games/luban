namespace Luban.Utils;

public static class ExceptionUtil
{
    public static string ExtractMessage(Exception e)
    {
        var lines = new List<string>();
        do
        {
            lines.Add("===> " + e.Message);
            e = e.InnerException;
        } while (e != null);

        return string.Join('\n', lines);
    }
}
