using System.Net.Http.Headers;
using System.Net.Http.Json;
using Gym.Application.Contracts.CheckIns;
using Gym.WebUI.Services.Authentication;

namespace Gym.WebUI.Services.CheckIns;

public sealed class AuthenticatedCheckInApiClient(HttpClient httpClient, IAccessTokenProvider accessTokenProvider)
    : ICheckInApiClient
{
    public async Task<CheckInApiResponse> CheckInAsync(
        Guid tenantId,
        Guid branchId,
        CheckInApiRequest request,
        CancellationToken cancellationToken = default)
    {
        var accessToken = await accessTokenProvider.GetAccessTokenAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new InvalidOperationException("An access token is required to check in a member.");
        }

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"api/tenants/{tenantId}/branches/{branchId}/check-ins")
        {
            Content = JsonContent.Create(request)
        };
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CheckInApiResponse>(cancellationToken)
            ?? throw new InvalidOperationException("The check-in API returned an empty response.");
    }
}
