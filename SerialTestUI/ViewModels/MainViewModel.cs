using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MySerialPort;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SerialTestUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Properties
        public SerialPortService SerialPortService { get; set; }

        #region SerialPortConfiguration
        private string _portName;
        public string PortName
        {
            get
            {
                return _portName;
            }
            set
            {
                Set<string>(() => PortName, ref _portName, value);
            }
        }
        private string _baudRate;
        public string BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                Set<string>(() => BaudRate, ref _baudRate, value);
            }
        }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Handshake Handshake { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
        #endregion

        private string _outGoingComm;
        public string OutGoingComm
        {
            get
            {
                return _outGoingComm;
            }
            set
            {
                Set<string>(() => OutGoingComm, ref _outGoingComm, value);
            }
        }
        private string _inGoingComm;
        public string InGoingComm
        {
            get
            {
                return _inGoingComm;
            }
            set
            {
                Set<string>(() => InGoingComm, ref _inGoingComm, value);
            }
        }

        public RelayCommand Start { get; set; }
        public RelayCommand Stop { get; set; }
        public RelayCommand LedToggle { get; set; }
        #endregion

        public MainViewModel()
        {
            SerialPortService = new SerialPortService();

            SerialPortService.DataReceived += (s) =>
            {
                InGoingComm = s + Environment.NewLine + InGoingComm;
            };
            SerialPortService.DataSend += (s) => {
                OutGoingComm = s + Environment.NewLine + OutGoingComm;
            };

            #region Commands
            Start = new RelayCommand(() =>
            {
                SerialPortService.Start();
            });
            Stop = new RelayCommand(() =>
            {
                SerialPortService.Stop();
            });
            LedToggle = new RelayCommand(() =>
            {
                SerialPortService.SendData("SWITCH_ON_LIGHT");
            });
            #endregion
        }

        #region Methods

        #endregion
    }
}
