namespace Gym.WebUI.Services.Ui;

public interface IToastService
{
    event Action? Changed;
    IReadOnlyList<ToastMessage> Messages { get; }
    void Show(string message, ToastTone tone = ToastTone.Info, string? title = null);
    void Dismiss(Guid id);
}

public enum ToastTone
{
    Info,
    Success,
    Warning,
    Danger
}

public sealed record ToastMessage(Guid Id, string Message, ToastTone Tone, string? Title);
