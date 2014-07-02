using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
                Set<string>(() => PortName, ref _portName, value, true);
            }
        }
        private int _baudRate;
        public int BaudRate
        {
            get
            {
                return _baudRate;
            }
            set
            {
                Set<int>(() => BaudRate, ref _baudRate, value, true);
            }
        }
        private Parity _parity;
        public Parity Parity
        {
            get
            {
                return _parity;
            }
            set
            {
                Set<Parity>(() => Parity, ref _parity, value, true);
            }
        }
        private int _dataBits;
        public int DataBits
        {
            get
            {
                return _dataBits;
            }
            set
            {
                Set<int>(() => DataBits, ref _dataBits, value, true);
            }
        }
        private StopBits _stopBits;
        public StopBits StopBits
        {
            get
            {
                return _stopBits;
            }
            set
            {
                Set<StopBits>(() => StopBits, ref _stopBits, value, true);
            }
        }
        private Handshake _handShake;
        public Handshake HandShake
        {
            get
            {
                return _handShake;
            }
            set
            {
                Set<Handshake>(() => HandShake, ref _handShake, value, true);
            }
        }
        private int _readTimeout;
        public int ReadTimout
        {
            get
            {
                return _readTimeout;
            }
            set
            {
                Set<int>(() => ReadTimout, ref _readTimeout, value, true);
            }
        }
        private int _writeTimeout;
        public int WriteTimeout
        {
            get
            {
                return _writeTimeout;
            }
            set
            {
                Set<int>(() => WriteTimeout, ref _writeTimeout, value, true);
            }
        }
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

            #region SerialPortConfiguration
            this.PortName = SerialPortService.PortName;
            this.BaudRate = SerialPortService.BaudRate;
            this.Parity = SerialPortService.Parity;
            this.DataBits = SerialPortService.DataBits;
            this.StopBits = SerialPortService.StopBits;
            this.HandShake = SerialPortService.Handshake;
            this.ReadTimout = SerialPortService.ReadTimeout;
            this.WriteTimeout = SerialPortService.WriteTimeout;

            Messenger.Default.Register<PropertyChangedMessage<string>>(this, (a) => {
                if (a.PropertyName == "PortName")
                {
                    SerialPortService.PortName = a.NewValue;
                }
            });
            Messenger.Default.Register<PropertyChangedMessage<int>>(this, (a) =>
            {
                if (a.PropertyName == "BaudRate")
                {
                    SerialPortService.BaudRate = a.NewValue;
                }
                else if (a.PropertyName == "DataBits")
                {
                    SerialPortService.DataBits = a.NewValue;
                }
            });
            Messenger.Default.Register<PropertyChangedMessage<Parity>>(this, (a) =>
            {
                SerialPortService.Parity = a.NewValue;
            });
            Messenger.Default.Register<PropertyChangedMessage<StopBits>>(this, (a) =>
            {
                SerialPortService.StopBits = a.NewValue;
            });
            Messenger.Default.Register<PropertyChangedMessage<Handshake>>(this, (a) =>
            {
                SerialPortService.Handshake = a.NewValue;
            });
            #endregion

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
