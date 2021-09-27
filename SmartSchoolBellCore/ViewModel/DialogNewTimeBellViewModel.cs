using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartSchoolBellCore.ViewModel
{
    public class DialogNewTimeBellViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DateTime TimeBellText { get; set; }

        public string TextName { get; set; }

        public Action EventCancel;
        public ICommand ComCancel
        {
            get
            {
                return new DelegateCommand(o=> EventCancel());
            }
        }

        public Action<int,int> EventNewTimeBell;
        public ICommand ComNewTimeBell { get; set; }
        public bool IsEnabledButtonCreate { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DialogNewTimeBellViewModel()
        {

        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                switch (columnName)
                {
                    case nameof(TextName):
                        if (string.IsNullOrWhiteSpace(TextName))
                        {
                            result = "Пустое поле";
                            IsEnabledButtonCreate = false;
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComNewTimeBell = new DelegateCommand(o => { });
                            OnPropertyChanged(nameof(ComNewTimeBell));
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComNewTimeBell = new DelegateCommand( o 
                                => EventNewTimeBell(TimeBellText.Hour, TimeBellText.Minute));
                            OnPropertyChanged(nameof(ComNewTimeBell));
                        }
                        break;
                    case nameof(TimeBellText):
                        if (TimeBellText == new DateTime())
                        {
                            result = "Пустое поле";
                            IsEnabledButtonCreate = false;
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComNewTimeBell = new DelegateCommand(o => { });
                            OnPropertyChanged(nameof(ComNewTimeBell));
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComNewTimeBell = new DelegateCommand(o
                                => EventNewTimeBell(TimeBellText.Hour, TimeBellText.Minute));
                            OnPropertyChanged(nameof(ComNewTimeBell));
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get; }
    }
}