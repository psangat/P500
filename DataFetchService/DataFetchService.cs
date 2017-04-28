using Agilent.AtomicSpectroscopy.ICP.Protocol.Scp;
using Agilent.Nexus.Protocol.Scp;
using Agilent.Nexus.ScpClient.Connection;
using Agilent.Nexus.ScpClient.PublicInterfaces;
using Agilent.Nexus.ScpClient.ScpHandler;
using Agilent.Nexus.ScpClient.SLIP;
using Agilent.Nexus.Xdr;
using DataFetchService;
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

      
         
      
        public async Task<AsyncSsrfStatus> getSSRFMeasurements()
        {
            AsyncSsrfStatus _ssrfStatus = new AsyncSsrfStatus();
            await new Task(()=>
            {
                try
                {
                    if (_instrumentConnection.State == ConnectionState.Connected)
                    {
                        DirectSlipServer _server = new DirectSlipServer(_diagnosticPort);
                        AsyncAlarmStatus _alarmStatus = new AsyncAlarmStatus();

                        _ssrfStatus.Configure(_server, SubsystemID);

                        while (_ssrfStatus.StatusUpdated <= 0)
                        {
                            // wait for status update
                        }
                        // _alarmStatus.Configure(_server, SubsystemID);

                        if (_ssrfStatus.StatusUpdated > 0)
                        {
                            Console.WriteLine("Dac0FeedbackVolts: {0}", _ssrfStatus.Dac0FeedBackVolts);
                            Console.WriteLine("HVPSVolts: {0}", _ssrfStatus.HVPSVolts);
                            Console.WriteLine("HVPSAmps: {0}", _ssrfStatus.HVPSAmps);
                            Console.WriteLine("HVPSSymmetryVolts: {0}", _ssrfStatus.HVPSSymmetryVolts);
                            Console.WriteLine("Temp1Celsius: {0}", _ssrfStatus.Temp1Celsius);
                            Console.WriteLine("Temp2Celsius: {0}", _ssrfStatus.Temp2Celsius);
                            Console.WriteLine("Temp3Celsius: {0}", _ssrfStatus.Temp3Celsius);
                            Console.WriteLine("DogBoneCelsius: {0}", _ssrfStatus.DogBoneCelsius);
                            Console.WriteLine("WorkCoilAmps: {0}", _ssrfStatus.WorkCoilAmps);
                            Console.WriteLine("WorkCoilHertz: {0}", _ssrfStatus.WorkCoilHertz);
                            Console.WriteLine("OscillatorHertz: {0}", _ssrfStatus.OscillatorHertz);
                            Console.WriteLine("OscillatorHertz2: {0}", _ssrfStatus.OscillatorHertz2);
                            Console.WriteLine("AirFlowHertz: {0}", _ssrfStatus.AirFlowHertz);
                            Console.WriteLine("CurrentPowerWatts: {0}", _ssrfStatus.CurrentPowerWatts);
                            Console.WriteLine("====================================================");

                        }
                    }
                    else
                    {
                        Console.WriteLine("Instrument not connected. Please connect instrument first.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in getting SSRF Measurements");
                    Console.WriteLine(ex.Message);

                }
               
            });
            return _ssrfStatus;
        }
    }
}
