namespace Smart_school_bell.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyMigration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Timetables", "Working", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Timetables", "Working");
        }
    }
}
