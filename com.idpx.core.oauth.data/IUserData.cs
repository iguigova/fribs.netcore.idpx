using System.Collections.Generic;

namespace com.idpx.core.oauth.data
{
    public interface IUserData
    {
        bool AuthSucceeded { get; }
        string AccessToken { get; } 
        string TokenType { get; }
        long? ExpiresIn { get; } // in seconds
        string RefreshToken { get; }
        long? RefreshTokenExpiresIn { get; } // in seconds
        string IdentityToken { get; }
        string ReturnedScopes { get; }
        string UID { get; set; }
        string Email { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Avatar { get; set; }

        Dictionary<string, string> Claims { get; set; }

        string Error { get; }
        string ErrorMessage { get; }

        void ParseUserData(string uid, string email, string firstName, string lastName = null, string avatar = null);
    }
}
