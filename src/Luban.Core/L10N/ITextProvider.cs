namespace Luban.L10N;

public interface ITextProvider
{
    bool Enable { get; }
    
    void Load();
    
    bool IsValidKey(string key);

    string GetText(string key, string language);
}