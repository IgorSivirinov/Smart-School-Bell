using Microsoft.EntityFrameworkCore;

namespace SmartSchoolBellCore.Model
{
    public sealed class DatabaseContext : DbContext
    {

        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<TimetableDayOfWeek> TimetableDayOfWeeks { get; set; }
        public DbSet<TimeHourMin> TimeHourMins { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<PasswordData> Passwords { get; set; }

        private readonly string _connectString = "Data Source=start_timer_bell.db";

        public DatabaseContext()
        {
            Database.EnsureCreated();
        }
        
        public DatabaseContext(string connectString)
        {
            _connectString = connectString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Timetable>()
                .HasMany(e => e.TimetableDayOfWeeks)
                .WithOne(e => e.Timetable);

            modelBuilder.Entity<TimetableDayOfWeek>()
                .HasMany(e => e.TimeBells)
                .WithOne(e => e.TimetableDayOfWeek);
          
            base.OnModelCreating(modelBuilder);
        }
    }
}