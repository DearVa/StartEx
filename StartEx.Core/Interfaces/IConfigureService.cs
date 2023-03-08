namespace StartEx.Core.Interfaces;

public interface IConfigureService
{
    void Save(string key, object value);

    object? Load(string key, object? defaultValue = default);

    bool LoadBoolean(string key, bool defaultValue = default);

    int LoadInt(string key, int defaultValue = default);

    float LoadFloat(string key, float defaultValue = default);

    double LoadDouble(string key, double defaultValue = default);

    string? LoadString(string key, string? defaultValue = default);

    bool Delete(string key);
}