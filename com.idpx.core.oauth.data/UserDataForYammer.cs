namespace com.idpx.core.oauth.data
{
    public class UserDataForYammer : UserData, IUserData
    {
        public override string AccessToken { get { return _data?.access_token?.token; } }

        public UserDataForYammer()
        {
        }

        public UserDataForYammer(string data) : base(data)
        {
        }
    }
}
