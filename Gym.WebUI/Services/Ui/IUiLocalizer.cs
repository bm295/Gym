namespace Gym.WebUI.Services.Ui;

public interface IUiLocalizer
{
    event Action? Changed;
    string Language { get; }
    string this[string key] { get; }
    void SetLanguage(string language);
}
