using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartSchoolBellCore.ViewModel
{
    public class DialogDeleteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelButton { get; set; }
        public event Action EventClickCancelButton;

        public ICommand YesButton { get; set; }
        public event Action EventClickYesButton;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DialogDeleteViewModel()
        {
            CancelButton = new DelegateCommand(o=>
            {
                EventClickCancelButton();
            });
            YesButton = new DelegateCommand(o => EventClickYesButton());
        }
        

    }
}