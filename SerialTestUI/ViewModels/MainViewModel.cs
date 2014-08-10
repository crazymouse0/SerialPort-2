using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using MySerialPort;
using MySerialPort.Models;
using Newtonsoft.Json;
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
        private string _errorBox;
        public string ErrorBox
        {
            get
            {
                return _errorBox;
            }
            set
            {
                Set<string>(() => ErrorBox, ref _errorBox, value);
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
                OutGoingComm = String.Empty;
                InGoingComm = String.Empty;
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
                    SerialPortService.SendData(new CommunicationModel() { Command = "REGNER", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "REGNER", Action = "OFF" });
            });
            SprueherToggle = new RelayCommand(() =>
            {
                if (!SprueherOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "SPRUEHER", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "SPRUEHER", Action = "OFF" });
            });
            TropferToggle = new RelayCommand(() =>
            {
                if (!TropferOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "TROPFER", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "TROPFER", Action = "OFF" });
            });
            ManualToggle = new RelayCommand(() =>
            {
                if (!ManualOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "MANUAL", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "MANUAL", Action = "OFF" });
            });
            OnRelaisToggle = new RelayCommand(() =>
            {
                if (!OnRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "ONRELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "ONRELAIS", Action = "OFF" });
            });
            OneRelaisToggle = new RelayCommand(() =>
            {
                if (!OneRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "ONERELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "ONERELAIS", Action = "OFF" });
            });
            TwoRelaisToggle = new RelayCommand(() =>
            {
                if (!TwoRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "TWORELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "TWORELAIS", Action = "OFF" });
            });
            ThreeRelaisToggle = new RelayCommand(() =>
            {
                if (!ThreeRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "THREERELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "THREERELAIS", Action = "OFF" });
            });
            VentilRelaisToggle = new RelayCommand(() =>
            {
                if (!VentilRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "VENTILRELAIS", Action = "WATER" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "VENTILRELAIS", Action = "ZISTERNE" });
            });
            ManualRelaisToggle = new RelayCommand(() =>
            {
                if (!ManualRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "MANUALRELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "MANUALRELAIS", Action = "OFF" });
            });
            PumpeRelaisToggle = new RelayCommand(() =>
            {
                if (!PumpeRelaisOn)
                    SerialPortService.SendData(new CommunicationModel() { Command = "PUMPERELAIS", Action = "ON" });
                else
                    SerialPortService.SendData(new CommunicationModel() { Command = "PUMPERELAIS", Action = "OFF" });
            });
            #endregion
        }
        private void analyseReturnCommand(string command)
        {
            try
            {
                if (command == String.Empty) return;

                var comData = JsonConvert.DeserializeObject<CallbackCommunicationModel>(command);

                if (comData.Type == "INFO")
                {
                    if (comData.Command == "REGNER" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.RegnerOn = true;
                    }
                    else if (comData.Command == "REGNER" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.RegnerOn = false;
                    }
                    else if (comData.Command == "SPRUEHER" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.SprueherOn = true;
                    }
                    else if (comData.Command == "SPRUEHER" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.SprueherOn = false;
                    }
                    else if (comData.Command == "TROPFER" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.TropferOn = true;
                    }
                    else if (comData.Command == "TROPFER" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.TropferOn = false;
                    }
                    else if (comData.Command == "ONRELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.OnRelaisOn = true;
                    }
                    else if (comData.Command == "ONRELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.OnRelaisOn = false;
                    }
                    else if (comData.Command == "ONERELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.OneRelaisOn = true;
                    }
                    else if (comData.Command == "ONERELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.OneRelaisOn = false;
                    }
                    else if (comData.Command == "TWORELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.TwoRelaisOn = true;
                    }
                    else if (comData.Command == "TWORELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.TwoRelaisOn = false;
                    }
                    else if (comData.Command == "THREERELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.ThreeRelaisOn = true;
                    }
                    else if (comData.Command == "THREERELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.ThreeRelaisOn = false;
                    }
                    else if (comData.Command == "VENTILRELAIS" && comData.Action == "WATER" && comData.Status == "OK")
                    {
                        this.VentilRelaisOn = true;
                    }
                    else if (comData.Command == "VENTILRELAIS" && comData.Action == "ZISTERNE" && comData.Status == "OK")
                    {
                        this.VentilRelaisOn = false;
                    }
                    else if (comData.Command == "MANUALRELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.ManualRelaisOn = true;
                    }
                    else if (comData.Command == "MANUALRELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.ManualRelaisOn = false;
                    }
                    else if (comData.Command == "PUMPERELAIS" && comData.Action == "ON" && comData.Status == "OK")
                    {
                        this.PumpeRelaisOn = true;
                    }
                    else if (comData.Command == "PUMPERELAIS" && comData.Action == "OFF" && comData.Status == "OK")
                    {
                        this.PumpeRelaisOn = false;
                    }
                }
                else if (comData.Type == "ERROR")
                {
                    ErrorBox = "Error on device: " + comData.Message;
                }
                //else if (com == "INFO")
                //{
                //    if (action == "WASSERSTAND_OK")
                //    {
                //        this.WasserstandOk = true;
                //    }
                //    else if (action == "WASSERSTAND_NOK")
                //    {
                //        this.WasserstandOk = false;
                //    }
                //    else if (action == "MANUAL_PRESSED")
                //    {
                //        this.HandTasterPressed = true;
                //    }
                //    else if (action == "MANUAL_NOT_PRESSED")
                //    {
                //        this.HandTasterPressed = false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                ErrorBox = ex.Message;
            }
        }
        #region Methods

        #endregion
    }
}
