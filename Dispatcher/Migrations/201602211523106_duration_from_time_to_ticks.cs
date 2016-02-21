namespace Dispatcher.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class duration_from_time_to_ticks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DispatchRequests", "DurationTicks", c => c.Long(nullable: false));
            AddColumn("dbo.DispatchRequests", "ServiceDurationTicks", c => c.Long(nullable: false));
            DropColumn("dbo.DispatchRequests", "Duration");
            DropColumn("dbo.DispatchRequests", "ServiceDuration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DispatchRequests", "ServiceDuration", c => c.Time(precision: 7));
            AddColumn("dbo.DispatchRequests", "Duration", c => c.Time(precision: 7));
            DropColumn("dbo.DispatchRequests", "ServiceDurationTicks");
            DropColumn("dbo.DispatchRequests", "DurationTicks");
        }
    }
}
