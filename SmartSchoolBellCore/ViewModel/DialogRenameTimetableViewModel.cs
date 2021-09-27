using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartSchoolBellCore.ViewModel
{
    public class DialogRenameTimetableViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event Action<string> EventRename;
        public ICommand ComRename { get; set; }

        public event Action EventCancel;
        public ICommand ComCancel { get; set; }

        public string TextName { get; set; }

        public bool IsEnabledButtonCreate { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DialogRenameTimetableViewModel(string name)
        {
            ComCancel = new DelegateCommand(o=> EventCancel());
            TextName = name;
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
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComRename = new DelegateCommand(o=>{});
                            OnPropertyChanged(nameof(ComRename));
                        }
                        else
                        {
                            IsEnabledButtonCreate = true;
                            OnPropertyChanged(nameof(IsEnabledButtonCreate));
                            ComRename = new DelegateCommand(o => EventRename(TextName));
                            OnPropertyChanged(nameof(ComRename));
                        }
                        break;
                }

                return result;
            }
        }

        public string Error { get; }
    }
}