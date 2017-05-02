using System;
using System.Net;
using System.Net.Sockets;

namespace DataFetchService
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.SetOut(new ConsoleWriter());
            Console.WriteLine("Starting Data Fetch Service for P500...");
            var ipAddress = "146.223.0.238";

            
            var dataFetchService = new DataFetchService();
            dataFetchService.connectToInstrument(ipAddress);

            Socket sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 146.223.0.73
            // 10.57.115.133
            IPAddress sendToAddress = IPAddress.Parse("10.57.115.133");
            IPEndPoint sendingEndPoint = new IPEndPoint(sendToAddress, 4444);
            Console.WriteLine("Sending the subsystem control status as UDP packets.");

            while (true)
            {
                string ssrfStatus = dataFetchService.getSSRFStatus();
                dataFetchService.sendStatus(ssrfStatus, sendingSocket, sendingEndPoint);
                Console.WriteLine(ssrfStatus);
            }

        }
    }
}
