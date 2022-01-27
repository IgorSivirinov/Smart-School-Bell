using ReactiveUI;

namespace SmartSchoolBell.Uno.ViewModels
{
    public class TimetableViewModel : ReactiveObject
    {
        private bool _isWorkingTimetable = false;

        public bool IsWorkingTimetable
        {
            get => _isWorkingTimetable;
            set => this.RaiseAndSetIfChanged(ref _isWorkingTimetable, value);
        }
    }
}