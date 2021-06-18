using System.Linq;

namespace com.idpx.core.oauth.data
{
    public class UserDataForPaypal : UserData, IUserData
    {
        private string _uid;
        public override string UID 
        { 
            get { return _uid; } 

            set
            {
                _uid = value.Split("/").Last();
            }
        }

        public UserDataForPaypal()
        {
        }

        public UserDataForPaypal(string data) : base(data)
        {
        }
    }
}
