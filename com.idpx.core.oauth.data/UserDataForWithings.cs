namespace com.idpx.core.oauth.data
{
    public class UserDataForWithings : UserData, IUserData
    {
        public override string UID { get { return _data?.userid; } }

        public UserDataForWithings()
        {
        }

        public UserDataForWithings(string data) : base(data)
        {
        }
    }
}
