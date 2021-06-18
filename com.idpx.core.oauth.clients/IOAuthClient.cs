using System.Threading.Tasks;
using com.idpx.core.oauth.data;
using Microsoft.AspNetCore.Http;

namespace com.idpx.core.oauth.clients
{
    public interface IOAuthClient : IApiClient
    {
        Task AuthorizeAsync(HttpResponse response);

        Task<string> GetTokenAsync(string code, params string[] state);

        Task<string> RefreshTokenAsync(string token);

        Task RevokeTokenAsync(string token);

        bool CanRefreshToken { get; }
        bool CanRevokeToken { get; }

        Task<IUserData> GetUserDataAsync(string data);
    }
}
