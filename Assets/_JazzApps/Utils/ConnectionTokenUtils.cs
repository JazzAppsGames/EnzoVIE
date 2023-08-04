using System;

namespace JazzApps.Utils
{
    /// <summary>
    /// Fusion Connection Token Utility methods
    /// </summary>
    public static class ConnectionTokenUtils
    {
        private static bool hasGeneratedToken = false; 
        private static byte[] generatedToken; // TODO: set/get from some file source so we can reconnect on game crash :)
        /// <summary>
        /// Create new random Token
        /// </summary>
        public static byte[] NewToken(bool singleton)
        {
            switch (singleton)
            {
                case true when !hasGeneratedToken:
                case false:
                    generatedToken = Guid.NewGuid().ToByteArray();
                    break;
            }
            hasGeneratedToken = true;
            return generatedToken;
        } 

        /// <summary>
        /// Converts a Token into a Hash format
        /// </summary>
        /// <param name="token">Token to be hashed</param>
        /// <returns>Token hash</returns>
        public static int HashToken(byte[] token) => new Guid(token).GetHashCode();
    }
}