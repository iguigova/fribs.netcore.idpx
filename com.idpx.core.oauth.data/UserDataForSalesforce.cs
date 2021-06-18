using System.Linq;

namespace com.idpx.core.oauth.data
{
    public class UserDataForSalesforce : UserData, IUserData
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

        public UserDataForSalesforce()
        {
        }

        public UserDataForSalesforce(string data) : base(data)
        {
        }
    }
}
