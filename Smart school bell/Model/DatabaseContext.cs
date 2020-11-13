using System.Data.Entity;

namespace Smart_school_bell.Model
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DatabaseConnect") { }

        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<TimetableDayOfWeek> TimetableDayOfWeeks { get; set; }
        public DbSet<History> Histories { get; set; }

    }
}