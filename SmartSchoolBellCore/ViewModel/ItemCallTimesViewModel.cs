using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SmartSchoolBellCore.ViewModel
{
    public class ItemCallTimesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event Action<int> DialogDeleteEvent;

        public bool IsButtonAdd { get; set; }

        public string Time { get; set; }

        public Action EventDialogNewTimeBell;

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
                return new DelegateCommand(o => DialogDeleteEvent(_listId));
            }
        }

        private readonly int _listId;

        public string Text { get; set; }

        public ItemCallTimesViewModel(TimeSpan time, int listId)
        {
            Text += time.Hours < 10 ? "0" + time.Hours + ":" : time.Hours + ":";
            Text += time.Minutes < 10 ? "0"+time.Minutes : ""+time.Minutes;
            _listId = listId;
        }

        public ItemCallTimesViewModel(bool isButtonAdd)
        {
            IsButtonAdd = isButtonAdd;
        }
    }
}