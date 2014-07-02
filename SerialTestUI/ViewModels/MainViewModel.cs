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
using System.Windows.Data;
using System.Windows.Input;

namespace SerialTestUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _firstTime = true;

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
        
        public CollectionView PortNames { get; set; }
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
        private bool _ledOn;
        public bool LedOn
        {
            get
            {
                return _ledOn;
            }
            set
            {
                Set<bool>(() => LedOn, ref _ledOn, value);
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
                var com = s.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                InGoingComm = com + Environment.NewLine + InGoingComm;

                //Check Commands
                analyseReturnCommand(com);
            };
            SerialPortService.DataSend += (s) => {
                OutGoingComm = s + Environment.NewLine + OutGoingComm;
            };

            #region SerialPortConfiguration
            this.PortNames = new CollectionView(SerialPortService.AvailableCOMPorts);

            this.PortName = SerialPortService.PortName;

            Messenger.Default.Register<PropertyChangedMessage<string>>(this, (a) => {
                if (a.PropertyName == "PortName")
                {
                    SerialPortService.PortName = a.NewValue;
                }
            });
            #endregion

            #region Commands
            Start = new RelayCommand(() =>
            {
                SerialPortService.Start();
            }, () => { 
                return !SerialPortService.ComIsOpen;
            });
            Stop = new RelayCommand(() =>
            {
                SerialPortService.Stop();
            }, () => {
                return SerialPortService.ComIsOpen;
            });
            LedToggle = new RelayCommand(() =>
            {
                if (!LedOn)
                    SerialPortService.SendData("SWITCH_ON_LIGHT");
                else
                    SerialPortService.SendData("SWITCH_OFF_LIGHT");
            });
            #endregion
        }
        private void analyseReturnCommand(string command)
        {
            //Aufbau <command>:<action>
            var ps = command.Split(':');
            var com = ps[0].Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            var action = ps[1].Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            if (com == "SWITCH_ON_LIGHT" && action == "EXECUTED")
            {
                this.LedOn = true;
            }
            else if (com == "SWITCH_OFF_LIGHT" && action == "EXECUTED")
            {
                this.LedOn = false;
            }
        }
        #region Methods

        #endregion
    }
}
