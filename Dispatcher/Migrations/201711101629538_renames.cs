namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renames : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DispatchRequests", newName: "Requests");
            RenameTable(name: "dbo.DispatchRequestTypes", newName: "RequestTypes");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.RequestTypes", newName: "DispatchRequestTypes");
            RenameTable(name: "dbo.Requests", newName: "DispatchRequests");
        }
    }
}
