using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Data.Excel
{
    public class SqlQueryDto
    {
        public string ConnectionString { get; set; }
        public string SqlCommand { get; set; }
        public int CommandTimeout { get; set; }
        public Dictionary<string, object>? CommandParameter { get; set; }
        

        public SqlQueryDto(string con, string cmd, int timeout = 300)
        {
            ConnectionString = con;
            SqlCommand = cmd;            
            CommandTimeout = timeout;
        }
    }
}
