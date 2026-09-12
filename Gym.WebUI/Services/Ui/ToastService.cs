namespace Gym.WebUI.Services.Ui;

public sealed class ToastService : IToastService
{
    private readonly List<ToastMessage> _messages = [];

    public event Action? Changed;
    public IReadOnlyList<ToastMessage> Messages => _messages;

    public void Show(string message, ToastTone tone = ToastTone.Info, string? title = null)
    {
        _messages.Add(new ToastMessage(Guid.NewGuid(), message, tone, title));
        Changed?.Invoke();
    }

    public void Dismiss(Guid id)
    {
        if (_messages.RemoveAll(message => message.Id == id) > 0)
        {
            Changed?.Invoke();
        }
    }
}
