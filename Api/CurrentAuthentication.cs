using System;

namespace Api
{
    public class CurrentAuthentication
    {
        public int UserId { get; private set; }
        public string AuthenticationToken { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime Expires { get; private set; }

        public bool Expired
        {
            get
            {
                if (string.IsNullOrEmpty(AuthenticationToken))
                    return false;
                return Expires <= DateTime.Now;
            }
        }

        public void Update(TokenUpdate response)
        {
            UserId = Convert.ToInt32(response.UserId);
            AuthenticationToken = response.AccessToken;
            RefreshToken = response.RefreshToken;
            Expires = response.ExpiresAt.DateTime;
        }
    }
}