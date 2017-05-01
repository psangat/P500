using Agilent.AtomicSpectroscopy.ICP.Protocol.Scp;
using Agilent.Nexus.Protocol.Scp;
using Agilent.Nexus.ScpClient.Connection;
using Agilent.Nexus.ScpClient.PublicInterfaces;
using Agilent.Nexus.ScpClient.ScpHandler;
using Agilent.Nexus.ScpClient.SLIP;
using Agilent.Nexus.Xdr;
using DataFetchService;
using Newtonsoft.Json;
using RFControlAsyncApp;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;


namespace DataFetchService
{
    class DataFetchService
    {
        private ProtocolSender _packetSender;
        private ProtocolReceiver _packetReceiver;
        private ConnectionTcp _instrumentConnection;
        private AuroraDiagnosticPort _diagnosticPort;
        private const int _serverPort = 8765;

        public void connect_To_Instrument(string ipAddress)
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
            return getSSRFStatusJSONString().GetAwaiter().GetResult();
        }

        public async Task<string> getSSRFStatusJSONString()
        {
            string result = string.Empty;
            await new Task(() =>
            {
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
            });
            return result;
        }

        public string getAlarmStatus()
        {
            return getAlarmStatusJSONString().GetAwaiter().GetResult();
        }

        public async Task<string> getAlarmStatusJSONString()
        {
            string result = string.Empty;
            await new Task(() =>
            {
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
            });
            return result;
        }
    }
}
