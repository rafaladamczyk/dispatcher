namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ValidRequesters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequestTypes", "ValidRequestersBytes", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DispatchRequestTypes", "ValidRequestersBytes");
        }
    }
}
