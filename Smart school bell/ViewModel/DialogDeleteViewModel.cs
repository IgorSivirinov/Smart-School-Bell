using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Smart_school_bell.Annotations;

namespace Smart_school_bell.ViewModel
{
    public class DialogDeleteViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand CancelButton { get; set; }
        public event Action EventClickCancelButton;

        public ICommand YesButton { get; set; }
        public event Action EventClickYesButton;
        

        [NotifyPropertyChangedInvocator]
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