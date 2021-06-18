using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace com.idpx.core.oauth.data
{
    public class UserData : IUserData
    {
        public virtual bool AuthSucceeded { get { return !string.IsNullOrEmpty(AccessToken); } }
        public virtual string AccessToken { get { return _data?.access_token; } } // case EIdPxTypes.yammer: return _result.access_token?.token;
        public virtual string TokenType { get { return _data?.token_type; } }
        public virtual long? ExpiresIn { get { return _data?.expires_in; } } // in seconds
        public virtual string RefreshToken { get { return _data?.refresh_token; } }
        public virtual long? RefreshTokenExpiresIn { get { return null; } } // in seconds
        public virtual string IdentityToken { get { return _data?.id_token; } }
        public virtual string ReturnedScopes { get { return _data?.scopes?.ToString() ?? _data?.scope?.ToString(); } } // twitch returns an array rather than a string: _"[\r\n  \"openid\",\r\n  \"user:read:email\"\r\n]"

        public virtual string UID { get; set; }
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Avatar { get; set; }

        public virtual Dictionary<string, string> Claims { get; set;}
        public string this[string claim] { get { return (Claims.ContainsKey(claim)) ? Claims[claim] : null; } set { Claims[claim] = value; } }

        public virtual string Error { get { return _data?.error; } }
        public virtual string ErrorMessage { get { return _data?.error_description; } }

        protected dynamic _data; 

        public UserData()
        {

        }

        public UserData(string data)
        {
            _data = JsonConvert.DeserializeObject(data);

            //ParseAccessToken();
            ParseIdentityToken();
        }

        protected virtual void AppendClaims(JwtSecurityToken token)
        {
            Claims ??= new Dictionary<string, string>();

            foreach(var claim in token.Claims)
            {
                if (!Claims.ContainsKey(claim.Type.ToLower()))
                {
                    Claims.Add(claim.Type.ToLower(), claim.Value.Trim());
                }
            }
        }

        protected virtual void ParseAccessToken()
        {
            if (!string.IsNullOrEmpty(AccessToken))
            {
                AppendClaims(new JwtSecurityToken(AccessToken));
            }
        }

        public virtual void ParseIdentityToken()
        {
            if (!string.IsNullOrEmpty(IdentityToken))
            {
                AppendClaims(new JwtSecurityToken(IdentityToken));

                UID = this["oid"] ?? this["sub"];
                Email = this["upn"] ?? this["email"] ?? this["preferred_username"];

                ParseName(this["preferred_username"]);
                FirstName = this["given_name"];
                LastName = this["family_name"];
                Avatar = this["picture"]; // https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            }
        }

        public virtual void ParseUserData(string uid, string email, string firstName, string lastName = null, string avatar = null)
        {
            if (!string.IsNullOrEmpty(uid))
            {
                UID = uid;
            }

            if (!string.IsNullOrEmpty(email))
            {
                Email = email;
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                FirstName = firstName;
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                LastName = lastName;
            }

            if (string.IsNullOrEmpty(LastName))
            {
                ParseName(FirstName);
            }

            if (string.IsNullOrEmpty(Avatar))
            {
                Avatar = avatar;
            }
        }

        public virtual void ParseName(string name)
        {
            ParseName(name, out string firstname, out string lastname);

            FirstName = firstname;
            LastName = lastname;
        }

        protected virtual void ParseName(string name, out string firstname, out string lastname)
        {
            firstname = name;
            lastname = null;

            if (!string.IsNullOrEmpty(name))
            {
                var lastnameIndex = name?.IndexOf(' ') ?? 0;

                if (lastnameIndex > 0)
                {
                    firstname = name.Substring(0, lastnameIndex);
                    lastname = name.Substring(lastnameIndex + 1);
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
