using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public class Encrypter : IEncrypter
    {
        public string GetHash(string value, string salt)
        {
            var dBytes = new Rfc2898DeriveBytes(value, GetBytes(salt), 1000);
            return Convert.ToBase64String(dBytes.GetBytes(50));
        }

        private static byte[] GetBytes(string salt)
        {
            return Encoding.UTF8.GetBytes(salt);
        }

        public string GetSalt()
        {
            var salt = new byte[50];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }
    }
}
