using System;
using Microsoft.Xaml.Behaviors.Media;

namespace Smart_school_bell.Model
{
    public class History
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Change { get; set; }

        public History(){}

        public History(DateTime date, string change)
        {
            Date = date;
            Change = change;
        }
    }
}