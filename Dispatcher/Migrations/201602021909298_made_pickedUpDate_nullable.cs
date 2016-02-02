namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class made_pickedUpDate_nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DispatchRequests", "PickedUpDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DispatchRequests", "PickedUpDate", c => c.DateTime(nullable: false));
        }
    }
}
