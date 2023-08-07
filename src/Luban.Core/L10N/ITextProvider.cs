namespace Luban.L10N;

public interface ITextProvider
{
    void Load();
    
    bool IsValidKey(string key);

    string GetText(string key, string language);
}