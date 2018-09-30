using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nop.Data.Migrations
{
    public partial class Notifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueuedNotification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriorityId = table.Column<int>(nullable: false),
                    UserIds = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(maxLength: 1000, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    AttachmentFilePath = table.Column<string>(nullable: true),
                    AttachmentFileName = table.Column<string>(nullable: true),
                    AttachedDownloadId = table.Column<int>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    DontSendBeforeDateUtc = table.Column<DateTime>(nullable: true),
                    SentTries = table.Column<int>(nullable: false),
                    SentOnUtc = table.Column<DateTime>(nullable: true),
                    ObserverIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedNotification", x => x.Id);
                });

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
            migrationBuilder.DropTable(
                name: "QueuedNotification");

            migrationBuilder.DeleteData("ScheduleTask", "Name", "Notification");
        }
    }
}
