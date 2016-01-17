namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MadeCompletionDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DispatchRequests", "CompletionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DispatchRequests", "CompletionDate", c => c.DateTime(nullable: false));
        }
    }
}
