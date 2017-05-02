﻿using Agilent.AtomicSpectroscopy.ICP.Protocol.Scp;
using Agilent.Nexus.ScpClient.Connection;
using Agilent.Nexus.ScpClient.PublicInterfaces;
using Agilent.Nexus.ScpClient.ScpHandler;
using Agilent.Nexus.ScpClient.SLIP;
using Newtonsoft.Json;
using RFControlAsyncApp;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace DataFetchService
{
    class DataFetchService
    {
        private ProtocolSender _packetSender;
        private ProtocolReceiver _packetReceiver;
        private ConnectionTcp _instrumentConnection;
        private AuroraDiagnosticPort _diagnosticPort;
        private const int _serverPort = 8765;

        public void connectToInstrument(string ipAddress)
        {
            _instrumentConnection = new ConnectionTcp(ipAddress, _serverPort);
            _packetSender = new ProtocolSender(_instrumentConnection);
            _packetReceiver = new ProtocolReceiver(_instrumentConnection);
            ProtocolAuroraResponseMapper.InitialiseAuroraResponseMap(_packetReceiver);
            _diagnosticPort = new AuroraDiagnosticPort(_packetSender, _packetReceiver);

            try
            {
                Console.WriteLine("Connecting to {0}", ipAddress);
                if (_instrumentConnection.State == ConnectionState.Connected)
                {
                    _diagnosticPort.Close();
                    _instrumentConnection.Disconnect();
                }

                if (_instrumentConnection.Connect(ipAddress, _serverPort))
                {
                    _diagnosticPort.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in instrument connection");
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public int SubsystemID
        {
            get { return 0x60; }
        }

       

        public string getSSRFStatus()
        {
            string result = string.Empty;
            
                try
                {
                    if (_instrumentConnection.State == ConnectionState.Connected)
                    {
                        DirectSlipServer _server = new DirectSlipServer(_diagnosticPort);
                        AsyncSsrfStatus _ssrfStatus = new AsyncSsrfStatus();

                        _ssrfStatus.Configure(_server, SubsystemID);
                        while (_ssrfStatus.StatusUpdated <= 0)
                        {
                            // wait for status update
                        }
                        result = JsonConvert.SerializeObject(_ssrfStatus);
                        _server.Close();
                    }
                    else
                    {
                        Console.WriteLine("Instrument not connected. Please connect instrument first.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in getting SSRF Status");
                    Console.WriteLine(ex.Message);
                }
            
            return result;
        }

        public string getAlarmStatus()
        {
            string result = string.Empty;
           
                try
                {
                    if (_instrumentConnection.State == ConnectionState.Connected)
                    {
                        DirectSlipServer _server = new DirectSlipServer(_diagnosticPort);
                        AsyncAlarmStatus _alarmStatus = new AsyncAlarmStatus();

                        _alarmStatus.Configure(_server, SubsystemID);
                        while (_alarmStatus.StatusUpdated <= 0)
                        {
                            // wait for status update
                        }
                        result = JsonConvert.SerializeObject(_alarmStatus);
                        _server.Close();
                    }
                    else
                    {
                        Console.WriteLine("Instrument not connected. Please connect instrument first.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in getting Alarm Status");
                    Console.WriteLine(ex.Message);
                }
            return result;
        }

        public void sendStatus(string text_to_send, Socket sending_socket, IPEndPoint sending_end_point)
        {
            // the socket object must have an array of bytes to send.
            // this loads the string entered by the user into an array of bytes.
            byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);

            try
            {
                sending_socket.SendTo(send_buffer, sending_end_point);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
