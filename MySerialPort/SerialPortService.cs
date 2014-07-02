using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySerialPort
{
    public class SerialPortService
    {
        private SerialPort _serialPort;
        private volatile bool _continue;
        private Thread _listenThread;

        #region Properties
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Handshake Handshake { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        public List<string> AvailableCOMPorts { get; set; }
        public List<string> AvailableParities { get; set; }
        public List<string> AvailableStopBits { get; set; }
        public List<string> AvailableHandshakes { get; set; }
        #endregion

        #region Events
        public event Action<string> DataReceived;
        public event Action<string> DataSend;
        #endregion

        public SerialPortService()
        {
            _serialPort = new SerialPort();
            setDefaultValues();

            _listenThread = new Thread(listen) { IsBackground = true };
        }

        #region Methods
        public void Start()
        {
            prepareForStart();
            _continue = true;
            _serialPort.Open();
            _listenThread.Start();
        }
        public void Stop()
        {
            _continue = false;
            _listenThread.Join();
            _serialPort.Close();
        }
        public void SendData(string data)
        {
            _serialPort.WriteLine(data);
            DataSend(data);
        }
        private void listen()
        {
            while (_continue)
            {
                try
                {
                    string msg = _serialPort.ReadLine();
                    if (!String.IsNullOrEmpty(msg))
                    {
                        DataReceived(msg);
                    }
                }
                catch (TimeoutException)
                {
                }
            }
        }
        private void setDefaultValues()
        {
            if (_serialPort == null) return;

            this.PortName = _serialPort.PortName;
            this.BaudRate = _serialPort.BaudRate;
            this.Parity = _serialPort.Parity;
            this.DataBits = _serialPort.DataBits;
            this.StopBits = _serialPort.StopBits;
            this.Handshake = _serialPort.Handshake;


            this.WriteTimeout = 500;
            this.ReadTimeout = 500;
            _serialPort.ReadTimeout = this.ReadTimeout;
            _serialPort.WriteTimeout = this.WriteTimeout;

            this.AvailableCOMPorts = SerialPort.GetPortNames().ToList();
            this.AvailableParities = Enum.GetNames(typeof(Parity)).ToList();
            this.AvailableStopBits = Enum.GetNames(typeof(StopBits)).ToList();
            this.AvailableHandshakes = Enum.GetNames(typeof(Handshake)).ToList();
        }
        private void prepareForStart()
        {
            _serialPort.PortName = this.PortName;
            _serialPort.BaudRate = this.BaudRate;
            _serialPort.Parity = this.Parity;
            _serialPort.DataBits = this.DataBits;
            _serialPort.StopBits  = this.StopBits;
            _serialPort.Handshake = this.Handshake;

            _serialPort.ReadTimeout = this.ReadTimeout;
            _serialPort.WriteTimeout = this.WriteTimeout;
        }
        #endregion
    }
}
