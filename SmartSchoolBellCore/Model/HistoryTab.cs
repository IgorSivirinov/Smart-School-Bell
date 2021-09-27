using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Xaml.Behaviors.Media;

namespace SmartSchoolBellCore.Model
{
    public class History
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Change { get; set; }

        public History(){}

        public History(DateTime date, string change)
        {
            Date = date.ToString("G");
            Change = change;
        }

        public static async Task GetToDatabaseAsync(DatabaseContext context, History history)
        {
            await context.Histories.AddAsync(history);
            await context.SaveChangesAsync();
        }
    }
}