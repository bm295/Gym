using System.Threading;
using System.Threading.Tasks;

namespace Gym.Application
{
    public interface IExternalApiService
    {
        Task<string> GetExchangeTokenAsync(
            string tokenExchangeEndpoint,
            string subjectToken,
            string audience,
            string scope,
            CancellationToken cancellationToken = default);

        Task<string> CallExternalApiAsync(
            string apiUrl,
            string exchangeToken,
            CancellationToken cancellationToken = default);
    }
}
