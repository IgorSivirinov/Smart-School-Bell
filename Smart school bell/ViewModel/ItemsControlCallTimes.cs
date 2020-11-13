using System.Windows;
using System.Windows.Controls;

namespace Smart_school_bell.ViewModel
{
    public class ItemsControlCallTimes : DataTemplateSelector
    {
        public DataTemplate ItemCallTimes { get; set; }
        public DataTemplate ButtonAddItemCallTimes { get; set; }

        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            if (((ItemCallTimesViewModel)item).IsButtonAdd) return ButtonAddItemCallTimes;
            return ItemCallTimes;
        }
    }
}