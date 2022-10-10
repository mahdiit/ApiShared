using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public class XSecurityHeaderValidator : ISecurityHeaderValidator
    {
        public bool IsValid(string headerValue, out Dictionary<string, string> headers)
        {
            headers = new Dictionary<string, string>();

            try
            {
                var s = VEncrypt.Decrypt(headerValue, StaticParameters.Security.ApiHeader.EncryptKey);
                var sPart = s.Split('|');

                if (sPart.Length > 0)
                    headers.Add(StaticParameters.Security.ApiHeader.Partycode, sPart[0]);
                if (sPart.Length > 1)
                    headers.Add(StaticParameters.Security.ApiHeader.Username, sPart[1]);
                if (sPart.Length > 2)
                    headers.Add(StaticParameters.Security.ApiHeader.IPAddress, sPart[2]);
                if (sPart.Length > 3)
                    headers.Add(StaticParameters.Security.ApiHeader.Time, sPart[3]);
                if (sPart.Length > 4)
                    headers.Add(StaticParameters.Security.ApiHeader.Company, sPart[4]);
                if (sPart.Length > 5)
                    headers.Add(StaticParameters.Security.ApiHeader.Branch, sPart[5]);
                if (sPart.Length > 6)
                    headers.Add(StaticParameters.Security.ApiHeader.FiscalYear, sPart[6]);
                if (sPart.Length > 7)
                    headers.Add(StaticParameters.Security.ApiHeader.Userid, sPart[7]);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
