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
        private bool _regnerOn;
        public bool RegnerOn
        {
            get
            {
                return _regnerOn;
            }
            set
            {
                Set<bool>(() => RegnerOn, ref _regnerOn, value);
            }
        }
        private bool _sprueherOn;
        public bool SprueherOn
        {
            get
            {
                return _sprueherOn;
            }
            set
            {
                Set<bool>(() => SprueherOn, ref _sprueherOn, value);
            }
        }
        private bool _tropferOn;
        public bool TropferOn
        {
            get
            {
                return _tropferOn;
            }
            set
            {
                Set<bool>(() => TropferOn, ref _tropferOn, value);
            }
        }
        private bool _manualOn;
        public bool ManualOn
        {
            get
            {
                return _manualOn;
            }
            set
            {
                Set<bool>(() => ManualOn, ref _manualOn, value);
            }
        }
        private bool _onRelaisOn;
        public bool OnRelaisOn
        {
            get
            {
                return _onRelaisOn;
            }
            set
            {
                Set<bool>(() => OnRelaisOn, ref _onRelaisOn, value);
            }
        }
        private bool _ventilRelaisOn;
        public bool VentilRelaisOn
        {
            get
            {
                return _ventilRelaisOn;
            }
            set
            {
                Set<bool>(() => VentilRelaisOn, ref _ventilRelaisOn, value);
            }
        }
        private bool _manualRelaisOn;
        public bool ManualRelaisOn
        {
            get
            {
                return _manualRelaisOn;
            }
            set
            {
                Set<bool>(() => ManualRelaisOn, ref _manualRelaisOn, value);
            }
        }
        private bool _oneRelaisOn;
        public bool OneRelaisOn
        {
            get
            {
                return _oneRelaisOn;
            }
            set
            {
                Set<bool>(() => OneRelaisOn, ref _oneRelaisOn, value);
            }
        }
        private bool _twoRelaisOn;
        public bool TwoRelaisOn
        {
            get
            {
                return _twoRelaisOn;
            }
            set
            {
                Set<bool>(() => TwoRelaisOn, ref _twoRelaisOn, value);
            }
        }
        private bool _threeRelaisOn;
        public bool ThreeRelaisOn
        {
            get
            {
                return _threeRelaisOn;
            }
            set
            {
                Set<bool>(() => ThreeRelaisOn, ref _threeRelaisOn, value);
            }
        }
        private bool _pumpeRelaisOn;
        public bool PumpeRelaisOn
        {
            get
            {
                return _pumpeRelaisOn;
            }
            set
            {
                Set<bool>(() => PumpeRelaisOn, ref _pumpeRelaisOn, value);
            }
        }
        private bool _wasserstandOk;
        public bool WasserstandOk
        {
            get
            {
                return _wasserstandOk;
            }
            set
            {
                Set<bool>(() => WasserstandOk, ref _wasserstandOk, value);
            }
        }
        private bool _handTasterPressed;
        public bool HandTasterPressed
        {
            get
            {
                return _handTasterPressed;
            }
            set
            {
                Set<bool>(() => HandTasterPressed, ref _handTasterPressed, value);
            }
        }
        public RelayCommand Start { get; set; }
        public RelayCommand Stop { get; set; }
        public RelayCommand RegnerToggle { get; set; }
        public RelayCommand SprueherToggle { get; set; }
        public RelayCommand TropferToggle { get; set; }
        public RelayCommand ManualToggle { get; set; }
        public RelayCommand OnRelaisToggle { get; set; }
        public RelayCommand VentilRelaisToggle { get; set; }
        public RelayCommand ManualRelaisToggle { get; set; }
        public RelayCommand OneRelaisToggle { get; set; }
        public RelayCommand TwoRelaisToggle { get; set; }
        public RelayCommand ThreeRelaisToggle { get; set; }
        public RelayCommand PumpeRelaisToggle { get; set; }
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
            SerialPortService.DataSend += (s) =>
            {
                OutGoingComm = s + Environment.NewLine + OutGoingComm;
            };

            #region SerialPortConfiguration
            this.PortNames = new CollectionView(SerialPortService.AvailableCOMPorts);

            this.PortName = SerialPortService.PortName;

            Messenger.Default.Register<PropertyChangedMessage<string>>(this, (a) =>
            {
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
            }, () =>
            {
                return !SerialPortService.ComIsOpen;
            });
            Stop = new RelayCommand(() =>
            {
                SerialPortService.Stop();
            }, () =>
            {
                return SerialPortService.ComIsOpen;
            });
            RegnerToggle = new RelayCommand(() =>
            {
                if (!RegnerOn)
                    SerialPortService.SendData("REGNER_ON");
                else
                    SerialPortService.SendData("REGNER_OFF");
            });
            SprueherToggle = new RelayCommand(() =>
            {
                if (!SprueherOn)
                    SerialPortService.SendData("SPRUEHER_ON");
                else
                    SerialPortService.SendData("SPRUEHER_OFF");
            });
            TropferToggle = new RelayCommand(() =>
            {
                if (!TropferOn)
                    SerialPortService.SendData("TROPFER_ON");
                else
                    SerialPortService.SendData("TROPFER_OFF");
            });
            ManualToggle = new RelayCommand(() =>
            {
                if (!ManualOn)
                    SerialPortService.SendData("MANUAL_ON");
                else
                    SerialPortService.SendData("MANUAL_OFF");
            });
            OnRelaisToggle = new RelayCommand(() =>
            {
                if (!OnRelaisOn)
                    SerialPortService.SendData("ONRELAIS_ON");
                else
                    SerialPortService.SendData("ONRELAIS_OFF");
            });
            OneRelaisToggle = new RelayCommand(() =>
            {
                if (!OneRelaisOn)
                    SerialPortService.SendData("ONERELAIS_ON");
                else
                    SerialPortService.SendData("ONERELAIS_OFF");
            });
            TwoRelaisToggle = new RelayCommand(() =>
            {
                if (!TwoRelaisOn)
                    SerialPortService.SendData("TWORELAIS_ON");
                else
                    SerialPortService.SendData("TWORELAIS_OFF");
            });
            ThreeRelaisToggle = new RelayCommand(() =>
            {
                if (!ThreeRelaisOn)
                    SerialPortService.SendData("THREERELAIS_ON");
                else
                    SerialPortService.SendData("THREERELAIS_OFF");
            });
            VentilRelaisToggle = new RelayCommand(() =>
            {
                if (!VentilRelaisOn)
                    SerialPortService.SendData("VENTILRELAIS_WATER");
                else
                    SerialPortService.SendData("VENTILRELAIS_ZISTERNE");
            });
            ManualRelaisToggle = new RelayCommand(() =>
            {
                if (!ManualRelaisOn)
                    SerialPortService.SendData("MANUALRELAIS_ON");
                else
                    SerialPortService.SendData("MANUALRELAIS_OFF");
            });
            PumpeRelaisToggle = new RelayCommand(() =>
            {
                if (!PumpeRelaisOn)
                    SerialPortService.SendData("PUMPERELAIS_ON");
                else
                    SerialPortService.SendData("PUMPERELAIS_OFF");
            });
            #endregion
        }
        private void analyseReturnCommand(string command)
        {
            //Aufbau <command>:<action>
            var ps = command.Split(':');
            var com = ps[0].Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            var action = ps[1].Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            if (com == "REGNER_ON" && action == "EXECUTED")
            {
                this.RegnerOn = true;
            }
            else if (com == "REGNER_OFF" && action == "EXECUTED")
            {
                this.RegnerOn = false;
            }
            else if (com == "SPRUEHER_ON" && action == "EXECUTED")
            {
                this.SprueherOn = true;
            }
            else if (com == "SPRUEHER_OFF" && action == "EXECUTED")
            {
                this.SprueherOn = false;
            }
            else if (com == "TROPFER_ON" && action == "EXECUTED")
            {
                this.TropferOn = true;
            }
            else if (com == "TROPFER_OFF" && action == "EXECUTED")
            {
                this.TropferOn = false;
            }
            else if (com == "ONRELAIS_ON" && action == "EXECUTED")
            {
                this.OnRelaisOn = true;
            }
            else if (com == "ONRELAIS_OFF" && action == "EXECUTED")
            {
                this.OnRelaisOn = false;
            }
            else if (com == "ONERELAIS_ON" && action == "EXECUTED")
            {
                this.OneRelaisOn = true;
            }
            else if (com == "ONERELAIS_OFF" && action == "EXECUTED")
            {
                this.OneRelaisOn = false;
            }
            else if (com == "TWORELAIS_ON" && action == "EXECUTED")
            {
                this.TwoRelaisOn = true;
            }
            else if (com == "TWORELAIS_OFF" && action == "EXECUTED")
            {
                this.TwoRelaisOn = false;
            }
            else if (com == "THREERELAIS_ON" && action == "EXECUTED")
            {
                this.ThreeRelaisOn = true;
            }
            else if (com == "THREERELAIS_OFF" && action == "EXECUTED")
            {
                this.ThreeRelaisOn = false;
            }
            else if (com == "VENTILRELAIS_WATER" && action == "EXECUTED")
            {
                this.VentilRelaisOn = true;
            }
            else if (com == "VENTILRELAIS_ZISTERNE" && action == "EXECUTED")
            {
                this.VentilRelaisOn = false;
            }
            else if (com == "MANUALRELAIS_ON" && action == "EXECUTED")
            {
                this.ManualRelaisOn = true;
            }
            else if (com == "MANUALRELAIS_OFF" && action == "EXECUTED")
            {
                this.ManualRelaisOn = false;
            }
            else if (com == "PUMPERELAIS_ON" && action == "EXECUTED")
            {
                this.PumpeRelaisOn = true;
            }
            else if (com == "PUMPERELAIS_OFF" && action == "EXECUTED")
            {
                this.PumpeRelaisOn = false;
            }
            else if (com == "INFO")
            {
                if (action == "WASSERSTAND_OK")
                {
                    this.WasserstandOk = true;
                }
                else if (action == "WASSERSTAND_NOK")
                {
                    this.WasserstandOk = false;
                }
                else if (action == "MANUAL_PRESSED")
                {
                    this.HandTasterPressed = true;
                }
                else if (action == "MANUAL_NOT_PRESSED")
                {
                    this.HandTasterPressed = false;
                }
            }
        }
        #region Methods

        #endregion
    }
}
