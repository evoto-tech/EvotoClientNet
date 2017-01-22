using System;
using System.Runtime.Serialization;

namespace Api
{
    [Serializable]
    public class TokenUpdate
    {
        private int _expiresIn;

        /// <summary>
        ///     The newly provided access token.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; private set; }

        /// <summary>
        ///     The date and time at which the new access token expires.
        /// </summary>
        [IgnoreDataMember]
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        ///     The number of seconds until the new access token expires.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                ExpiresAt = DateTime.UtcNow.AddSeconds(_expiresIn);
            }
        }

        [DataMember(Name = "userId")]
        public string UserId { get; private set; }

        /// <summary>
        ///     The newly provided refresh token.
        /// </summary>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; private set; }
    }
}