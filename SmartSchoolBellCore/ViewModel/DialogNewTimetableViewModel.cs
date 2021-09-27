using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartSchoolBellCore.ViewModel
{
    public class DialogNewTimetableViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Action<string> EventNewItem;
        public ICommand ComNewItem { get; set; }

        public Action EventCancel;
        public ICommand ComCancel { get; set; }

        public bool IsEnabledButtonCreate { get; set; } = true;

        public string TextName { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DialogNewTimetableViewModel()
        {
            ComNewItem = new DelegateCommand(o=>
            {
                EventNewItem(TextName);
            });
            ComCancel = new DelegateCommand(o=> EventCancel());
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
                            ComNewItem = new DelegateCommand(o => { });
                            OnPropertyChanged("ComNewItem");
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged("IsEnabledButtonCreate");
                            ComNewItem = new DelegateCommand(o => EventNewItem(TextName));
                            OnPropertyChanged("ComNewItem");
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get; }

    }
}