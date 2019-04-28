namespace PreCheckUtility
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    public static class DpapiHandler
    {
        private static readonly byte[] sEntropy = { 1, 2, 3, 4, 5, 6 };

        public static string Encrypt(string text, DataProtectionScope scope)
        {
            // first, convert the text to byte array 
            byte[] originalText = Encoding.Unicode.GetBytes(text);

            // then use Protect() to encrypt your data 
            byte[] encryptedText = ProtectedData.Protect(originalText, sEntropy, scope);

            //and return the encrypted message 
            return Convert.ToBase64String(encryptedText);
        }

        public static string Decrypt(string text, DataProtectionScope scope)
        {
            // the encrypted text, converted to byte array 
            byte[] encryptedText = Convert.FromBase64String(text);

            // calling Unprotect() that returns the original text 
            byte[] originalText = ProtectedData.Unprotect(encryptedText, sEntropy, scope);

            // finally, returning the result 
            return Encoding.Unicode.GetString(originalText);
        }
    }
}
