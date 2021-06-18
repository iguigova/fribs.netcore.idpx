using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
    public class OAuth10aClientForTwitter : OAuth10aClient, IOAuthClient
    {
        public OAuth10aClientForTwitter(string requestTokenUrl, string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string redirectUrl, string stateId, string prompt, Action<string, string> onTokenUpdateAsync)
            : base(requestTokenUrl, loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, redirectUrl, stateId, prompt, onTokenUpdateAsync)
        {
        }

        //   // NOTE: we are missing the access_token_secret in generate the signing key for the signature
        //   public override void RevokeToken(string token)
        //   {
        //       base.RevokeToken(token);

        //       /*
        //        * https://developer.twitter.com/en/docs/basics/authentication/api-reference/invalidate_access_token
        //        * 
        //        * curl --request POST 
        // --url 'https://api.twitter.com/1.1/oauth/invalidate_token.json' 
        // --header 'authorization: OAuth oauth_consumer_key="CLIENT_KEY",
        //oauth_nonce="AUTO_GENERATED_NONCE", oauth_signature="AUTO_GENERATED_SIGNATURE",
        //oauth_signature_method="HMAC-SHA1", oauth_timestamp="AUTO_GENERATED_TIMESTAMP",
        //oauth_token="ACCESS_TOKEN", oauth_version="1.0"'
        //        */

        //       var signatureBaseString = string.Join('&', new List<string>()
        //          {
        //              "POST",
        //              Uri.EscapeDataString(_revokeTokenUrl),
        //              Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
        //              {
        //                  ( "oauth_token", token),
        //                  ( "oauth_consumer_key", _clientId ),
        //                  ( "oauth_nonce", _stateId.ToString() ),
        //                  ( "oauth_signature_method", "HMAC-SHA1" ),
        //                  ( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
        //                  ( "oauth_version", "1.0" )
        //              }).OrderBy(s => s.Item1))))
        //          });

        //       OnInfo?.Invoke(signatureBaseString);

        //       var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&{Uri.EscapeDataString(tokens.access_token_secret)}";

        //       var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

        //       OnInfo?.Invoke(signature);

        //       // https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

        //       var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
        //          {
        //              ( "oauth_token", token),
        //              ( "oauth_callback", _redirectUrl ),
        //              ( "oauth_consumer_key", _clientId ),
        //              ( "oauth_nonce", _stateId.ToString() ),
        //              ( "oauth_signature", signature),
        //              ( "oauth_signature_method", "HMAC-SHA1" ),
        //              ( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
        //              ( "oauth_version", "1.0" )
        //          }).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

        //       OnInfo?.Invoke(headerAuthorizationString);

        //       _client.Headers.Remove(HttpRequestHeader.Authorization);
        //       _client.Headers.Add(HttpRequestHeader.Authorization, $"OAuth {headerAuthorizationString}");

        //       Post($"{_revokeTokenUrl}");
        //   }

        protected override async Task<string> GetDataAsync(string url, string data)
        {
            var tokens = JsonConvert.DeserializeAnonymousType(data, new { access_token = (string)null, access_token_secret = (string)null });

            var signatureBaseString = string.Join('&', new List<string>()
            {
                "GET",
                Uri.EscapeDataString(_dataUrl),
                Uri.EscapeDataString(string.Join('&', ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
                {
                    ( "include_entities", "false"),
                    ( "skip_status", "true"),
                    ( "include_email", "true"),
                    ( "oauth_token", tokens.access_token),
                    ( "oauth_callback", _redirectUrl ),
                    ( "oauth_consumer_key", _clientId ),
                    ( "oauth_nonce", _stateId.ToString() ),
                    ( "oauth_signature_method", "HMAC-SHA1" ),
                    ( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
                    ( "oauth_version", "1.0" )
                }).OrderBy(s => s.Item1))))
            });

            //OnInfo?.Invoke(signatureBaseString);// is this safe to log? - JG

            var signingKey = $"{Uri.EscapeDataString(_clientSecret)}&{Uri.EscapeDataString(tokens.access_token_secret)}";

            var signature = CreateHMACSHA1Token(signatureBaseString, signingKey);

            //OnInfo?.Invoke(signature);// is this safe to log? - JG

            // https://developer.twitter.com/en/docs/basics/authentication/guides/authorizing-a-request

            var headerAuthorizationString = string.Join(", ", ZipDataTuples(EscapeDataTuples(new List<(string, string)>()
            {
                ( "oauth_token", tokens.access_token),
                ( "oauth_callback", _redirectUrl ),
                ( "oauth_consumer_key", _clientId ),
                ( "oauth_nonce", _stateId.ToString() ),
                ( "oauth_signature", signature),
                ( "oauth_signature_method", "HMAC-SHA1" ),
                ( "oauth_timestamp", DateTimeOffset.Now.ToUnixTimeSeconds().ToString() ),
                ( "oauth_version", "1.0" )
            }).OrderBy(s => s.Item1), (s) => { return $"\"{s}\""; }));

            return await GetAsync($"{_dataUrl}?include_entities=false&skip_status=true&include_email=true", new NameValueCollection
            {
                { HttpRequestHeader.Authorization.ToString(), $"OAuth {headerAuthorizationString}"},
            });
        }

        protected override void OnUserData(dynamic data)
        {
            #region Sample Response
            /*
                        {{
              "id": 1115661734180573184,
              "id_str": "1115661734180573184",
              "name": "LogonLabs Developer",
              "screen_name": "logonlabs",
              "location": "",
              "description": "",
              "url": null,
              "entities": {
                "description": {
                  "urls": []
                }
              },
              "protected": false,
              "followers_count": 0,
              "friends_count": 0,
              "listed_count": 0,
              "created_at": "Tue Apr 09 17:04:20 +0000 2019",
              "favourites_count": 0,
              "utc_offset": null,
              "time_zone": null,
              "geo_enabled": false,
              "verified": false,
              "statuses_count": 0,
              "lang": null,
              "contributors_enabled": false,
              "is_translator": false,
              "is_translation_enabled": false,
              "profile_background_color": "F5F8FA",
              "profile_background_image_url": null,
              "profile_background_image_url_https": null,
              "profile_background_tile": false,
              "avatar": "http://pbs.twimg.com/profile_images/1115663035572482049/SH6Cga2K_normal.jpg",
              "avatar_https": "https://pbs.twimg.com/profile_images/1115663035572482049/SH6Cga2K_normal.jpg",
              "profile_link_color": "1DA1F2",
              "profile_sidebar_border_color": "C0DEED",
              "profile_sidebar_fill_color": "DDEEF6",
              "profile_text_color": "333333",
              "profile_use_background_image": true,
              "has_extended_profile": false,
              "default_profile": true,
              "default_profile_image": false,
              "following": false,
              "follow_request_sent": false,
              "notifications": false,
              "translator_type": "none",
              "suspended": false,
              "needs_phone_verification": false,
              "email": "developer@logonlabs.com"
            }}
            */
            #endregion

            _userData.ParseUserData(data?.id?.Value.ToString(), data?.email?.Value, data?.name?.Value, data?.avatar_https?.Value ?? data?.avatar?.Value);
        }
    }
}