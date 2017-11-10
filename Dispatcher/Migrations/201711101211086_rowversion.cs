namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rowversion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequests", "RowVersion", c => c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"));
            DropColumn("dbo.DispatchRequests", "ProvidingUserName");
            DropColumn("dbo.DispatchRequests", "RequestingUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DispatchRequests", "RequestingUserName", c => c.String());
            AddColumn("dbo.DispatchRequests", "ProvidingUserName", c => c.String());
            DropColumn("dbo.DispatchRequests", "RowVersion");
        }
    }
}
