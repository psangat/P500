﻿using System;
using System.Threading;

namespace DataFetchService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetOut(new ConsoleWriter());
            Console.WriteLine("Starting Data Fetch Service for P500...");
            var ipAddress = "Invalid.IP.Address.Initialization";
            
            // Ask for IP address until user inputs valid IP address
            while (!Utils.isIPAddressValid(ipAddress))
            {
                Console.Write("Instrument IP address: ");
                ipAddress = Console.ReadLine();
            }

            Console.WriteLine("Connecting to {0}", ipAddress);
            for (int i = 0; i < 10; i++)
            {
                Console.Write(".");
                Thread.Sleep(1000);
            }
            Console.WriteLine();
            Console.WriteLine("Connected to {0}", ipAddress);
            Console.ReadKey();

        }
    }
}
