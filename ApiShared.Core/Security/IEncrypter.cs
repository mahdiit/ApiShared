using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public interface IEncrypter : ISingleton
    {
        string GetSalt();
        string GetHash(string value, string salt);
    }
}
