namespace Smart_school_bell.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TimetableDayOfWeeks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimetableId = c.Int(),
                        TimeBellJSON = c.String(),
                        DayOfWeekJSON = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Timetables", t => t.TimetableId)
                .Index(t => t.TimetableId);
            
            CreateTable(
                "dbo.Timetables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UriFile = c.String(),
                        СallDuration = c.Int(nullable: false),
                        Friday_Id = c.Int(),
                        Monday_Id = c.Int(),
                        Saturday_Id = c.Int(),
                        Thursday_Id = c.Int(),
                        Tuesday_Id = c.Int(),
                        Wednesday_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Friday_Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Monday_Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Saturday_Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Thursday_Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Tuesday_Id)
                .ForeignKey("dbo.TimetableDayOfWeeks", t => t.Wednesday_Id)
                .Index(t => t.Friday_Id)
                .Index(t => t.Monday_Id)
                .Index(t => t.Saturday_Id)
                .Index(t => t.Thursday_Id)
                .Index(t => t.Tuesday_Id)
                .Index(t => t.Wednesday_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimetableDayOfWeeks", "TimetableId", "dbo.Timetables");
            DropForeignKey("dbo.Timetables", "Wednesday_Id", "dbo.TimetableDayOfWeeks");
            DropForeignKey("dbo.Timetables", "Tuesday_Id", "dbo.TimetableDayOfWeeks");
            DropForeignKey("dbo.Timetables", "Thursday_Id", "dbo.TimetableDayOfWeeks");
            DropForeignKey("dbo.Timetables", "Saturday_Id", "dbo.TimetableDayOfWeeks");
            DropForeignKey("dbo.Timetables", "Monday_Id", "dbo.TimetableDayOfWeeks");
            DropForeignKey("dbo.Timetables", "Friday_Id", "dbo.TimetableDayOfWeeks");
            DropIndex("dbo.Timetables", new[] { "Wednesday_Id" });
            DropIndex("dbo.Timetables", new[] { "Tuesday_Id" });
            DropIndex("dbo.Timetables", new[] { "Thursday_Id" });
            DropIndex("dbo.Timetables", new[] { "Saturday_Id" });
            DropIndex("dbo.Timetables", new[] { "Monday_Id" });
            DropIndex("dbo.Timetables", new[] { "Friday_Id" });
            DropIndex("dbo.TimetableDayOfWeeks", new[] { "TimetableId" });
            DropTable("dbo.Timetables");
            DropTable("dbo.TimetableDayOfWeeks");
        }
    }
}
