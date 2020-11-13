using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;
using Smart_school_bell.View;

namespace Smart_school_bell.ViewModel
{
    public class ItemCallTimesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<int> DialogDeleteEvent;

        public bool IsButtonAdd { get; set; } = false;

        public string Time { get; set; }

        public event Action EventDialogNewTimeBell;

        public ICommand RunDialogAddItem
        {
            get
            {
                return new DelegateCommand(o=> EventDialogNewTimeBell());
            }
        }

        public ICommand DeleteItem
        {
            get
            {
                return new DelegateCommand(o => DialogDeleteEvent(ListId));
            }
        }

        private int ListId;

        public string Text { get; set; }

        public ItemCallTimesViewModel(TimeSpan time, int listId)
        {
            Text += time.Hours < 10 ? "0" + time.Hours + ":" : time.Hours + ":";
            Text += time.Minutes < 10 ? "0"+time.Minutes : ""+time.Minutes;
            ListId = listId;
        }

        public ItemCallTimesViewModel(bool isButtonAdd)
        {
            IsButtonAdd = isButtonAdd;
        }
    }
}