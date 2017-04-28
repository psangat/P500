using Agilent.AtomicSpectroscopy.ICP.Protocol.Scp;
using Agilent.Nexus.Protocol.Scp;
using Agilent.Nexus.ScpClient.Connection;
using Agilent.Nexus.ScpClient.PublicInterfaces;
using Agilent.Nexus.ScpClient.ScpHandler;
using Agilent.Nexus.ScpClient.SLIP;
using Agilent.Nexus.Xdr;
using RFControlAsyncApp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataFetchService
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.SetOut(new ConsoleWriter());
            Console.WriteLine("Starting Data Fetch Service for P500...");
            var ipAddress = "146.223.0.238";

            // Ask for IP address until user inputs valid IP address
            while (!Utils.isIPAddressValid(ipAddress))
            {
                Console.Write("Instrument IP address: {0}", ipAddress);
                //ipAddress = Console.ReadLine();
                Console.WriteLine();
            }
            var dataFetchService = new DataFetchService();
            dataFetchService.connect_To_Instrument(ipAddress);

            for (int i = 0; i < 10; i++)
            {
                Task<AsyncSsrfStatus> _xyz = dataFetchService.getSSRFMeasurements();
                // AsyncSsrfStatus _ssrfStatus = await _xyz.ConfigureAwait(fa);

            }

            Console.ReadKey();
        }
    }
}
