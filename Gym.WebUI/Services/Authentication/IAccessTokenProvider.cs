namespace Gym.WebUI.Services.Authentication;

public interface IAccessTokenProvider
{
    ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
