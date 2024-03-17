namespace Luban.L10N;

public interface ITextProvider
{
    void Load();

    void ProcessDatas();

    bool IsValidKey(string key);

    bool TryGetText(string key, out string text);

    void AddUnknownKey(string key);

    bool ConvertTextKeyToValue { get; }
}
