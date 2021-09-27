using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using SmartSchoolBellCore.Model;

namespace SmartSchoolBellCore.ViewModel
{
    public sealed class HistoryPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<History> DataGridHistoryItems { get; set; } = new();

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HistoryPageViewModel()
        {
            using var context = new DatabaseContext();
            foreach (var item in context.Histories)
            {
                DataGridHistoryItems.Add(item);
                DataGridHistoryItems.Reverse();
            }
            OnPropertyChanged(nameof(DataGridHistoryItems));
        }
    }
}