using com.idpx.core.oauth.data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForPlanningCenter : OAuth2Client<UserData>, IOAuthClient
    {
        public OAuth2ClientForPlanningCenter(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
            {  
               {  
                  "data":{  
                     "type":"Person",
                     "id":"56586865",
                     "attributes":{  
                        "accounting_administrator":true,
                        "anniversary":null,
                        "avatar":"https://people.planningcenteronline.com/static/no_photo_thumbnail_gray.png",
                        "birthdate":null,
                        "child":false,
                        "created_at":"2019-07-08T21:57:47Z",
                        "demographic_avatar_url":"https://people.planningcenteronline.com/static/no_photo_thumbnail_gray.png",
                        "first_name":"Test",
                        "gender":null,
                        "given_name":null,
                        "grade":null,
                        "graduation_year":null,
                        "inactivated_at":null,
                        "last_name":"Person",
                        "medical_notes":null,
                        "membership":null,
                        "middle_name":null,
                        "name":"Test Person",
                        "nickname":null,
                        "passed_background_check":false,
                        "people_permissions":null,
                        "remote_id":null,
                        "school_type":null,
                        "site_administrator":true,
                        "status":"active",
                        "updated_at":"2019-07-08T21:57:47Z"
                     },
                     "relationships":{  
                        "primary_campus":{  
                           "data":null
                        }
                     },
                     "links":{  
                        "":"https://api.planningcenteronline.com/people/v2/people/56586865/",
                        "addresses":"https://api.planningcenteronline.com/people/v2/people/56586865/addresses",
                        "apps":"https://api.planningcenteronline.com/people/v2/people/56586865/apps",
                        "connected_people":"https://api.planningcenteronline.com/people/v2/people/56586865/connected_people",
                        "emails":"https://api.planningcenteronline.com/people/v2/people/56586865/emails",
                        "field_data":"https://api.planningcenteronline.com/people/v2/people/56586865/field_data",
                        "household_memberships":"https://api.planningcenteronline.com/people/v2/people/56586865/household_memberships",
                        "households":"https://api.planningcenteronline.com/people/v2/people/56586865/households",
                        "inactive_reason":null,
                        "marital_status":null,
                        "message_groups":"https://api.planningcenteronline.com/people/v2/people/56586865/message_groups",
                        "messages":"https://api.planningcenteronline.com/people/v2/people/56586865/messages",
                        "name_prefix":null,
                        "name_suffix":null,
                        "notes":"https://api.planningcenteronline.com/people/v2/people/56586865/notes",
                        "person_apps":"https://api.planningcenteronline.com/people/v2/people/56586865/person_apps",
                        "phone_numbers":"https://api.planningcenteronline.com/people/v2/people/56586865/phone_numbers",
                        "platform_notifications":"https://api.planningcenteronline.com/people/v2/people/56586865/platform_notifications",
                        "primary_campus":null,
                        "school":null,
                        "social_profiles":"https://api.planningcenteronline.com/people/v2/people/56586865/social_profiles",
                        "workflow_cards":"https://api.planningcenteronline.com/people/v2/people/56586865/workflow_cards",
                        "self":"https://api.planningcenteronline.com/people/v2/people/56586865"
                     }
                  },
                  "included":[  

                  ],
                  "meta":{  
                     "can_include":[  
                        "addresses",
                        "emails",
                        "field_data",
                        "households",
                        "inactive_reason",
                        "marital_status",
                        "name_prefix",
                        "name_suffix",
                        "person_apps",
                        "phone_numbers",
                        "platform_notifications",
                        "primary_campus",
                        "school",
                        "social_profiles"
                     ],
                     "parent":{  
                        "id":"293345",
                        "type":"Organization"
                     }
                  }
               }
            }
             */
            #endregion

            var emailData = JsonConvert.DeserializeObject<dynamic>(await GetAsync(data?.data?.links?.emails?.ToString(), DataRequestHeaders));

            #region Sample Response
            /*
{  
{  
  "links":{  
     "self":"https://api.planningcenteronline.com/people/v2/people/56586865/emails"
  },
  "data":[  
     {  
        "type":"Email",
        "id":"35893217",
        "attributes":{  
           "address":"developer@logonlabs.com",
           "created_at":"2019-07-08T21:57:47Z",
           "location":"Work",
           "primary":true,
           "updated_at":"2019-07-08T21:57:47Z"
        },
        "relationships":{  
           "person":{  
              "data":{  
                 "type":"Person",
                 "id":"56586865"
              }
           }
        },
        "links":{  
           "self":"https://api.planningcenteronline.com/people/v2/emails/35893217"
        }
     }
  ],
  "included":[  

  ],
  "meta":{  
     "total_count":1,
     "count":1,
     "can_order_by":[  
        "address",
        "location",
        "primary",
        "created_at",
        "updated_at"
     ],
     "can_query_by":[  
        "address",
        "location",
        "primary"
     ],
     "parent":{  
        "id":"56586865",
        "type":"Person"
     }
  }
}
}
             */
            #endregion

            var emails = new List<dynamic>(emailData.data);
            var email = (string)null;

            foreach (var e in emails)
            {
                if (e?.attributes?.primary?.Value ?? false)
                {
                    email = e?.attributes?.address?.Value;
                    break;
                }
            }

            _userData.ParseUserData(data?.data?.id?.Value.ToString(), email, data?.data?.attributes?.first_name?.Value, data?.data?.attributes?.last_name?.Value, data?.data?.attributes?.avatar?.Value);
        }
    }
}