using com.idpx.core.oauth.data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
	public class OAuth2ClientForFoursquare : OAuth2Client<UserData>, IOAuthClient
	{
		public OAuth2ClientForFoursquare(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
			: base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
		{
		}

		public override HttpContent TokenRequest(string code)
		{
			return new FormUrlEncodedContent(new Dictionary<string, string>()
			{
				{ "grant_type", "authorization_code" },
				{ "code", code },
				{ "client_id", _clientId},
				{ "client_secret", _clientSecret},
				{ "redirect_uri", _redirectUrl}
			});
		}

		public override string DataRequest { get { return $"{_dataUrl}?oauth_token={_userData.AccessToken}&v={DateTime.Today:yyyymmdd}"; } }

		protected override async Task OnUserDataAsync(dynamic data)
		{
			#region Sample Data
			/*
{ 
   { 
      "meta":{ 
         "code":200,
         "requestId":"5d939764dd0f85001ba1316f"
      },
      "notifications":[ 
         { 
            "type":"notificationTray",
            "item":{ 
               "unreadCount":0
            }
         }
      ],
      "response":{ 
         "user":{ 
            "id":"554445240",
            "firstName":"Developer",
            "lastName":"LogonLabs",
            "gender":"none",
            "relationship":"self",
            "canonicalUrl":"https://foursquare.com/user/554445240",
            "photo":{ 
               "prefix":"https://fastly.4sqi.net/img/user/",
               "suffix":"/blank_boy.png",
               "default":true
            },
            "friends":{ 
               "count":0,
               "groups":[ 
                  { 
                     "type":"friends",
                     "name":"Mutual friends",
                     "count":0,
                     "items":[ 

                     ]
                  },
                  { 
                     "type":"others",
                     "name":"Other friends",
                     "count":0,
                     "items":[ 

                     ]
                  }
               ]
            },
            "birthday":-462672000,
            "tips":{ 
               "count":0
            },
            "homeCity":"Vancouver, BC",
            "bio":"",
            "contact":{ 
               "verifiedPhone":"false",
               "email":"developer@logonlabs.com"
            },
            "photos":{ 
               "count":0,
               "items":[ 

               ]
            },
            "checkinPings":"off",
            "pings":false,
            "type":"user",
            "mayorships":{ 
               "count":0,
               "items":[ 

               ]
            },
            "checkins":{ 
               "count":0,
               "items":[ 

               ]
            },
            "requests":{ 
               "count":0
            },
            "lists":{ 
               "count":2,
               "groups":[ 
                  { 
                     "type":"created",
                     "count":0,
                     "items":[ 

                     ]
                  },
                  { 
                     "type":"followed",
                     "count":0,
                     "items":[ 

                     ]
                  },
                  { 
                     "type":"yours",
                     "count":2,
                     "items":[ 
                        { 
                           "id":"554445240/todos",
                           "name":"My Saved Places",
                           "description":"",
                           "type":"todos",
                           "editable":true,
                           "public":true,
                           "collaborative":false,
                           "url":"/user/554445240/list/todos",
                           "canonicalUrl":"https://foursquare.com/user/554445240/list/todos",
                           "listItems":{ 
                              "count":0
                           }
                        },
                        { 
                           "id":"554445240/venuelikes",
                           "name":"My Liked Places",
                           "description":"",
                           "type":"likes",
                           "editable":true,
                           "public":true,
                           "collaborative":false,
                           "url":"/user/554445240/list/venuelikes",
                           "canonicalUrl":"https://foursquare.com/user/554445240/list/venuelikes",
                           "listItems":{ 
                              "count":0
                           }
                        }
                     ]
                  }
               ]
            },
            "blockedStatus":"none",
            "createdAt":1562710700,
            "lenses":[ 

            ],
            "referralId":"u-554445240"
         }
      }
   }
}
             */
			#endregion

			_userData.ParseUserData(data?.response?.user?.id.Value.ToString(), data?.response?.user?.contact?.email?.Value, data?.response?.user?.firstName?.Value, data?.response?.user?.lastName?.Value, data?.response?.user?.photo?.prefix + data?.response?.user?.photo?.sufix);
		}
	}
}