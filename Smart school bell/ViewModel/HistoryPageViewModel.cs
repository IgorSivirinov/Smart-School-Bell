using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using Smart_school_bell.Annotations;
using Smart_school_bell.Model;

namespace Smart_school_bell.ViewModel
{
    public class HistoryPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<History> DataGridHistoryItems
        {
            get;
            set;
        } = new List<History>();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HistoryPageViewModel()
        {
            using (var context = new DatabaseContext())
            {
                foreach (var item in context.Histories)
                {
                    DataGridHistoryItems.Add(item);
                    DataGridHistoryItems.Reverse();
                }
                OnPropertyChanged("DataGridHistoryItems");
            }
        }
    }
}