using System;

namespace Api.Exceptions
{
    public class TokenUpdateEventArgs : EventArgs
    {
        public TokenUpdateEventArgs(TokenUpdate tokens)
        {
            Tokens = tokens;
        }

        /// <summary>
        ///     Gets the refreshed access tokens.
        /// </summary>
        public TokenUpdate Tokens { get; private set; }
    }
}