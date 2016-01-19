namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_Duration_To_Request : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequests", "Duration", c => c.Time(precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DispatchRequests", "Duration");
        }
    }
}
