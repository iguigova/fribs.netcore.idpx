namespace com.idpx.core.oauth.data
{
    public class UserDataForApple : UserData, IUserData
    {
        public UserDataForApple()
        {
        }

        public UserDataForApple(string data) : base(data)
        {
        }

        public override void ParseIdentityToken()
        {
            base.ParseIdentityToken();

            // https://developer.apple.com/documentation/sign_in_with_apple/sign_in_with_apple_js/incorporating_sign_in_with_apple_into_other_platforms
            // { "name": { "firstName": string, "lastName": string }, "email": string }

            if (_data?.user != null)
            {
                Email ??= _data.user.email;
                FirstName ??= _data?.name?.firstName;
                LastName ??= _data?.name?.lastName;
            }
        }
    }
}
