namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_Active_flag_to_Requests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequests", "Active", c => c.Boolean(nullable: false));
            CreateIndex("dbo.DispatchRequests", "Active");
        }
        
        public override void Down()
        {
            DropIndex("dbo.DispatchRequests", new[] { "Active" });
            DropColumn("dbo.DispatchRequests", "Active");
        }
    }
}
