using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntugentBackend.Services.Data
{
    static class CLocalData
    {
        public static string Machine;
        public static string User;
        public static string Db_ConStr;
        public static string Host_Url;  //Used by Azure vault and Okta
        public static string feedBack;
    }
}
