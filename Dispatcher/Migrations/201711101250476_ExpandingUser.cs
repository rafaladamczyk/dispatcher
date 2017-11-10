namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandingUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Confirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "HourlyWages", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "HourlyWages");
            DropColumn("dbo.AspNetUsers", "Confirmed");
        }
    }
}
