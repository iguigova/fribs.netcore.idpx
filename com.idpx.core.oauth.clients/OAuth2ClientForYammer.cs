using com.idpx.core.oauth.data;
using System.Threading.Tasks;

namespace com.idpx.core.oauth.clients
{
    public class OAuth2ClientForYammer : OAuth2Client<UserDataForYammer>, IOAuthClient
    {
        public OAuth2ClientForYammer(string loginUrl, string tokenUrl, string revokeTokenUrl, string dataUrl, string clientId, string clientSecret, string scope, string redirectUrl, string stateId, string prompt = null, string hint = null)
            : base(loginUrl, tokenUrl, revokeTokenUrl, dataUrl, clientId, clientSecret, scope, redirectUrl, stateId, prompt: prompt, hint: hint)
        {
        }

        protected override async Task OnUserDataAsync(dynamic data)
        {
            #region Sample Response
            /*
{
"user":
{
"timezone": "Hawaii",
"interests": null,
"type": "user",
"mugshot_url": "https://www.yammer.com/yamage-backstage/photos/…",
"kids_names": null,
"settings": {
  "xdr_proxy": "https://stagexdrproxy.yammer.com"
},
"schools": [],
"verified_admin": "false",
"birth_date": "",
"expertise": null,
"job_title": "",
"state": "active",
"contact": {
  "phone_numbers": [],
  "im": {
    "provider": "",
    "username": ""
  },
  "email_addresses": [
    {
      "type": "primary",
      "address": "test@yammer-inc.com"
    }
  ]
},
"location": null,
"previous_companies": [],
"hire_date": null,
"admin": "false",
"full_name": "TestAccount",
"network_id": 155465488,
"stats": {
  "updates": 2,
  "followers": 0,
  "following": 0
},
"can_broadcast": "false",
"summary": null,
"external_urls": [],
"name": "clientappstest",
"network_domains": [
  "yammer-inc.com"
],
"network_name": "Yammer",
"significant_other": null,
"id": 1014216,
"web_url": "https://www.yammer.com/yammer-inc.com/users/…",
"url": "https://www.yammer.com/api/v1/users/101416",
"guid": null
},
"access_token": {
"view_subscriptions": true,
"expires_at": null,
"authorized_at": "2011/04/06 16:25:46 +0000",
"modify_subscriptions": true,
"modify_messages": true,
"network_permalink": "yammer-inc.com",
"view_members": true,
"view_tags": true,
"network_id": 155465488,
"user_id": 1014216,
"view_groups": true,
"token": "ajsdfiasd7f6asdf8o",
"network_name": "Yammer",
"view_messages": true,
"created_at": "2011/04/06 16:25:46 +0000"
},
"network": {
"type": "network",
"header_background_color": "#0092bc",
"community": false,
"navigation_background_color": "#3f5f9e",
"navigation_text_color": "#ffffff",
"permalink": "yammer-inc.com",
"paid": true,
"show_upgrade_banner": false,
"name": "Yammer",
"is_org_chart_enabled": true,
"id": 155465488,
"header_text_color": "#000000",
"web_url": "https://www.yammer.com/yammer-inc.com"
}
}                  
             */
            #endregion

            #region Sep 08 21:52:52 RD00155DD56676. LogonLabs.IdPx.API: 04:52:52 Info
            /*
{  
\"access_token\":{  
  \"user_id\":149260525568,
  \"network_id\":8576630784,
  \"network_permalink\":\"logonlabs.com\",
  \"network_name\":\"logonlabs.com\",
  \"network_canonical\":true,
  \"network_primary\":true,
  \"token\":\"8576630784-JxBx2C1CBqOvJ2yU71crHA\",
  \"view_members\":true,
  \"view_groups\":true,
  \"view_messages\":true,
  \"view_subscriptions\":true,
  \"modify_subscriptions\":true,
  \"modify_messages\":true,
  \"view_tags\":true,
  \"created_at\":      \"2019/09/09 04:52:51      +0000\",
  \"authorized_at\":      \"2019/09/09 04:52:51      +0000\",
  \"expires_at\":null
},
\"user\":{  
  \"type\":\"user\",
  \"id\":149260525568,
  \"network_id\":8576630784,
  \"state\":\"active\",
  \"guid\":null,
  \"job_title\":\"\",
  \"location\":null,
  \"interests\":null,
  \"summary\":null,
  \"expertise\":null,
  \"full_name\":\"Developer LogonLabs\",
  \"activated_at\":      \"2019/07/08 22:54:54      +0000\",
  \"auto_activated\":false,
  \"show_ask_for_photo\":true,
  \"first_name\":\"Developer\",
  \"last_name\":\"LogonLabs\",
  \"network_name\":\"logonlabs.com\",
  \"network_domains\":[  
     \"logonlabs.com\"
  ],
  \"url\":      \"https://www.yammer.com/api/v1/users/149260525568\",
  \"web_url\":      \"https://www.yammer.com/logonlabs.com/users/149260525568\",
  \"name\":\"developer\",
  \"mugshot_url\":      \"https://mug0.assets-yammer.com/mugshot/images/48x48/no_photo.png\",
  \"mugshot_url_template\":      \"https://mug0.assets-yammer.com/mugshot/images/      {  
     width
  }      x      {  
     height
  }      /no_photo.png\",
  \"birth_date\":\"\",
  \"birth_date_complete\":\"\",
  \"timezone\":\"Pacific Time (US \\u0026 Canada)\",
  \"external_urls\":[  

  ],
  \"admin\":\"false\",
  \"verified_admin\":\"false\",
  \"supervisor_admin\":\"false\",
  \"can_broadcast\":\"false\",
  \"department\":null,
  \"email\":\"developer@logonlabs.com\",
  \"guest\":false,
  \"updatable_profile\":true,
  \"can_create_new_network\":true,
  \"can_browse_external_networks\":true,
  \"significant_other\":\"\",
  \"kids_names\":\"\",
  \"previous_companies\":[  

  ],
  \"schools\":[  

  ],
  \"contact\":{  
     \"im\":{  
        \"provider\":\"\",
        \"username\":\"\"
     },
     \"phone_numbers\":[  

     ],
     \"email_addresses\":[  
        {  
           \"type\":\"primary\",
           \"address\":\"developer@logonlabs.com\"
        }
     ],
     \"has_fake_email\":false
  },
  \"stats\":{  
     \"following\":0,
     \"followers\":0,
     \"updates\":0
  },
  \"settings\":{  
     \"xdr_proxy\":\"\"
  },
  \"show_invite_lightbox\":false
},
\"network\":{  
  \"type\":\"network\",
  \"id\":8576630784,
  \"email\":\"logonlabs.com@yammer.com\",
  \"name\":\"logonlabs.com\",
  \"community\":false,
  \"permalink\":\"logonlabs.com\",
  \"web_url\":      \"https://www.yammer.com/logonlabs.com\",
  \"show_upgrade_banner\":false,
  \"header_background_color\":\"#396B9A\",
  \"header_text_color\":\"#FFFFFF\",
  \"navigation_background_color\":\"#38699F\",
  \"navigation_text_color\":\"#FFFFFF\",
  \"paid\":false,
  \"moderated\":false,
  \"is_freemium\":true,
  \"is_org_chart_enabled\":false,
  \"is_group_enabled\":true,
  \"is_chat_enabled\":true,
  \"is_translation_enabled\":false,
  \"created_at\":      \"2019/07/08 22:53:13      +0000\",
  \"profile_fields_config\":{  
     \"enable_work_history\":true,
     \"enable_education\":true,
     \"enable_job_title\":true,
     \"enable_work_phone\":true,
     \"enable_mobile_phone\":true,
     \"enable_summary\":true,
     \"enable_interests\":true,
     \"enable_expertise\":true,
     \"enable_location\":true,
     \"enable_im\":true,
     \"enable_skype\":true,
     \"enable_websites\":true
  },
  \"browser_deprecation_url\":null,
  \"external_messaging_state\":\"enabled\",
  \"state\":\"enabled\",
  \"enforce_office_authentication\":null,
  \"office_authentication_committed\":null,
  \"is_gif_shortcut_enabled\":true,
  \"is_link_preview_enabled\":true,
  \"attachments_in_private_messages\":true,
  \"secret_groups\":true,
  \"force_connected_groups\":false,
  \"connected_all_company\":false
}
}                  
             */
            #endregion

            _userData.ParseUserData(data?.user?.id?.Value.ToString(), data?.user?.email?.Value, data?.user?.first_name.Value, data?.user?.last_name?.Value, data?.user?.mugshot_url?.Value);
        }
    }
}