using System.Windows;
using System.Windows.Controls;

namespace SmartSchoolBellCore.ViewModel
{
    public class ItemsControlCallTimes : DataTemplateSelector
    {
        public DataTemplate ItemCallTimes { get; set; }
        public DataTemplate ButtonAddItemCallTimes { get; set; }

        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            if (((ItemCallTimesViewModel) item).IsButtonAdd) return ButtonAddItemCallTimes;
            return ItemCallTimes;
        }
    }
}