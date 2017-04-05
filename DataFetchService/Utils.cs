using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchService
{
    class Utils
    {

        public static bool isIPAddressValid(String ipAddress)
        {
            if (ipAddress.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length == 4)
            {
                IPAddress ip;
                return IPAddress.TryParse(ipAddress, out ip);
            }
            else
                return false;
        }
    }
}
