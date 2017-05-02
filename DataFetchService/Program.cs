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

            Console.WriteLine("Connecting to {0}", ipAddress);
            try
            {
                dataFetchService.connectToInstrument(ipAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in instrument connection");
                Console.WriteLine(ex.Message);
            }

            Socket sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // 146.223.0.73
            // 10.57.115.133
            IPAddress sendToAddress = IPAddress.Parse("10.57.115.133");
            IPEndPoint sendingEndPoint = new IPEndPoint(sendToAddress, 4000);
            Console.WriteLine("Sending the subsystem control status as UDP packets.");

            while (true)
            {
                try
                {
                    string ssrfStatus = dataFetchService.getSSRFStatus();
                    sendingEndPoint.Port = Constants.RF_PORT;
                    dataFetchService.sendStatus(ssrfStatus, sendingSocket, sendingEndPoint);
                    Console.WriteLine(ssrfStatus);

                    string alarmStatus = dataFetchService.getAlarmStatus();
                    sendingEndPoint.Port = Constants.ALARM_PORT;
                    dataFetchService.sendStatus(alarmStatus, sendingSocket, sendingEndPoint);
                    Console.WriteLine(alarmStatus);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in getting SSRF Status");
                    Console.WriteLine(ex.Message);
                }
                
            }

        }
    }
}
