using Agilent.AtomicSpectroscopy.ICP.Protocol.Scp;
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
                throw ex;
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
                    result = "Instrument not connected. Please connect instrument first.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
                
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
                    result = "Instrument not connected. Please connect instrument first.";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public void sendStatus(string textToSend, Socket sendingSocket, IPEndPoint sendingEndPoint)
        {
            byte[] send_buffer = Encoding.ASCII.GetBytes(textToSend);
            try
            {
                sendingSocket.SendTo(send_buffer, sendingEndPoint);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
