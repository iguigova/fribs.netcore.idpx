using com.idpx.core.oauth.data;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.idpx.core.oauth.clients
{
    public class OAuth10aClientForGoodreads : OAuth10aClient, IOAuthClient
    {
        public OAuth10aClientForGoodreads(string requestTokenUrl, string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string redirectUrl, string stateId, string prompt, Action<string, string> onTokenUpdateAsync)
            : base(requestTokenUrl, loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, redirectUrl, stateId, prompt, onTokenUpdateAsync)
        {
        }

        public async override Task<IUserData> GetUserDataAsync(string data)
        {
            _userData = new UserData(data);

            OnUserData(await GetDataAsync(_dataUrl, data)); // JsonConvert does not work here as it returns an xml

            return _userData;
        }

        protected override void OnUserData(dynamic data)
        {
            #region Sample Response
            /*
    <?xml version="1.0" encoding="UTF-8"?>
    <GoodreadsResponse>
    <Request>
    <authentication>true</authentication>
    <key><![CDATA[oonkhoutoYnZfNTyqbn9Q]]></key>
    <method><![CDATA[api_auth_user]]></method>
    </Request>
    <user id="99584980">
    <name>Developer LogonLabs</name>
    <link><![CDATA[https://www.goodreads.com/user/show/99584980-developer-logonlabs?utm_medium=api]]></link>
    </user>

    </GoodreadsResponse>                 
    */
            #endregion

            // https://stackoverflow.com/questions/13171525/converting-xml-to-a-dynamic-c-sharp-object
            data = JsonConvert.DeserializeObject(JsonConvert.SerializeXNode(XDocument.Parse(data)));

            #region Sample xml to json
            /*
{ 
   { 
      "?xml":{ 
         "@version":"1.0",
         "@encoding":"UTF-8"
      },
      "GoodreadsResponse":{ 
         "Request":{ 
            "authentication":"true",
            "key":{ 
               "#cdata-section":"oonkhoutoYnZfNTyqbn9Q"
            },
            "method":{ 
               "#cdata-section":"api_auth_user"
            }
         },
         "user":{ 
            "@id":"99584980",
            "name":"Developer LogonLabs",
            "link":{ 
               "#cdata-section":"https://www.goodreads.com/user/show/99584980-developer-logonlabs?utm_medium=api"
            }
         }
      }
   }
}
*/
            #endregion

            _userData.ParseUserData(data.GoodreadsResponse.user["@id"].Value, null, data.GoodreadsResponse.user.name.Value);
        }
    }
}