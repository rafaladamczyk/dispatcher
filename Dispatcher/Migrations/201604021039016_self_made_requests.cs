namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class self_made_requests : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequestTypes", "ForSelf", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DispatchRequestTypes", "ForSelf");
        }
    }
}
