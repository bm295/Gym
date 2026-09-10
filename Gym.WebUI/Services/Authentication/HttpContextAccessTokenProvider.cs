using Microsoft.AspNetCore.Http;

namespace Gym.WebUI.Services.Authentication;

public sealed class HttpContextAccessTokenProvider(IHttpContextAccessor httpContextAccessor) : IAccessTokenProvider
{
    public ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        var authorization = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization)
            || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return ValueTask.FromResult<string?>(null);
        }

        return ValueTask.FromResult<string?>(authorization["Bearer ".Length..].Trim());
    }
}
