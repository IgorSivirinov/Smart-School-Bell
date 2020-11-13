using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Smart_school_bell.Annotations;

namespace Smart_school_bell.ViewModel
{
    public class DialogNewTimeBellVM : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime TimeBallText { get; set; }

        public string TextName { get; set; }

        public Action EventCancel;
        public ICommand ComCancel
        {
            get
            {
                return new DelegateCommand(o=> EventCancel());
            }
        }

        public event Action<int,int> EventNewTimeBall;
        public ICommand ComNewTimeBall { get; set; }
        public bool IsEnabledButtonCreate { get; set; }



            [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DialogNewTimeBellVM()
        {

        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case "TextName":
                        if (string.IsNullOrWhiteSpace(TextName))
                        {
                            result = "Пустое поле";
                            IsEnabledButtonCreate = false;
                            OnPropertyChanged("IsEnabledButtonCreate");
                            ComNewTimeBall = new DelegateCommand(o => { });
                            OnPropertyChanged("ComNewTimeBall");
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged("IsEnabledButtonCreate");
                            ComNewTimeBall = new DelegateCommand(o 
                                => EventNewTimeBall(TimeBallText.Hour, TimeBallText.Minute));
                            OnPropertyChanged("ComNewTimeBall");
                        }
                        break;
                    case "TimeBallText":
                        if (TimeBallText == new DateTime())
                        {
                            result = "Пустое поле";
                            IsEnabledButtonCreate = false;
                            OnPropertyChanged("IsEnabledButtonCreate");
                            ComNewTimeBall = new DelegateCommand(o => { });
                            OnPropertyChanged("ComNewTimeBall");
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged("IsEnabledButtonCreate");
                            ComNewTimeBall = new DelegateCommand(o
                                => EventNewTimeBall(TimeBallText.Hour, TimeBallText.Minute));
                            OnPropertyChanged("ComNewTimeBall");
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get; }
    }
}