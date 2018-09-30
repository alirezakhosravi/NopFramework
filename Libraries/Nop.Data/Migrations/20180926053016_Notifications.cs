using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nop.Data.Migrations
{
    public partial class Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("ScheduleTask", new string[]
               { "Name", "Seconds", "Type", "Enabled", "StopOnError" },
               new object[]
               {
                    "Notification",
                    60,
                    "Nop.Services.Notifications.NotificationTask, Nop.Services",
                    true,
                    false
               }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "QueuedNotification");
        }
    }
}
