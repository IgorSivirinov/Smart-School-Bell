namespace Smart_school_bell.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyMigration2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Histories", "TypeChange");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Histories", "TypeChange", c => c.String());
        }
    }
}
