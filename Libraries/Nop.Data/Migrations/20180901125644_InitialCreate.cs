using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nop.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLogType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SystemKeyword = table.Column<string>(maxLength: 100, nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Enabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLogType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Subject = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    UserRoleId = table.Column<int>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    DontSendBeforeDateUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    TwoLetterIsoCode = table.Column<string>(maxLength: 2, nullable: true),
                    ThreeLetterIsoCode = table.Column<string>(maxLength: 3, nullable: true),
                    NumericIsoCode = table.Column<int>(nullable: false),
                    Published = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Download",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DownloadGuid = table.Column<Guid>(nullable: false),
                    UseDownloadUrl = table.Column<bool>(nullable: false),
                    DownloadUrl = table.Column<string>(nullable: true),
                    DownloadBinary = table.Column<byte[]>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: true),
                    Extension = table.Column<string>(nullable: true),
                    IsNew = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Download", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 255, nullable: true),
                    Host = table.Column<string>(maxLength: 255, nullable: false),
                    Port = table.Column<int>(nullable: false),
                    Username = table.Column<string>(maxLength: 255, nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    EnableSsl = table.Column<bool>(nullable: false),
                    UseDefaultCredentials = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    KeyGroup = table.Column<string>(maxLength: 400, nullable: false),
                    Key = table.Column<string>(maxLength: 400, nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    LanguageCulture = table.Column<string>(maxLength: 20, nullable: false),
                    UniqueSeoCode = table.Column<string>(maxLength: 2, nullable: true),
                    FlagImageFileName = table.Column<string>(maxLength: 50, nullable: true),
                    Rtl = table.Column<bool>(nullable: false),
                    Published = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    BccEmailAddresses = table.Column<string>(maxLength: 200, nullable: true),
                    Subject = table.Column<string>(maxLength: 1000, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    DelayBeforeSend = table.Column<int>(nullable: true),
                    DelayPeriodId = table.Column<int>(nullable: false),
                    AttachedDownloadId = table.Column<int>(nullable: false),
                    EmailAccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsLetterSubscription",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NewsLetterSubscriptionGuid = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLetterSubscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    SystemName = table.Column<string>(maxLength: 255, nullable: false),
                    Category = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MimeType = table.Column<string>(maxLength: 40, nullable: false),
                    SeoFilename = table.Column<string>(maxLength: 300, nullable: true),
                    AltAttribute = table.Column<string>(nullable: true),
                    TitleAttribute = table.Column<string>(nullable: true),
                    IsNew = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Seconds = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    StopOnError = table.Column<bool>(nullable: false),
                    LastStartUtc = table.Column<DateTime>(nullable: true),
                    LastEndUtc = table.Column<DateTime>(nullable: true),
                    LastSuccessUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchTerm",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Keyword = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchTerm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UrlRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    EntityName = table.Column<string>(maxLength: 400, nullable: false),
                    Slug = table.Column<string>(maxLength: 400, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserGuid = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(maxLength: 1000, nullable: true),
                    Email = table.Column<string>(maxLength: 1000, nullable: true),
                    EmailToRevalidate = table.Column<string>(maxLength: 1000, nullable: true),
                    AdminComment = table.Column<string>(nullable: true),
                    RequireReLogin = table.Column<bool>(nullable: false),
                    FailedLoginAttempts = table.Column<int>(nullable: false),
                    CannotLoginUntilDateUtc = table.Column<DateTime>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    IsSystemAccount = table.Column<bool>(nullable: false),
                    SystemName = table.Column<string>(maxLength: 400, nullable: true),
                    LastIpAddress = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    LastLoginDateUtc = table.Column<DateTime>(nullable: true),
                    LastActivityDateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    IsSystemRole = table.Column<bool>(nullable: false),
                    SystemName = table.Column<string>(maxLength: 255, nullable: true),
                    EnablePasswordLifetime = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AddressAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressAttributeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    IsPreSelected = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AddressAttributeValue_AddressAttribute_AddressAttributeId",
                        column: x => x.AddressAttributeId,
                        principalTable: "AddressAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StateProvince",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CountryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(maxLength: 100, nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StateProvince", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StateProvince_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueuedEmail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PriorityId = table.Column<int>(nullable: false),
                    From = table.Column<string>(maxLength: 500, nullable: false),
                    FromName = table.Column<string>(maxLength: 500, nullable: true),
                    To = table.Column<string>(maxLength: 500, nullable: false),
                    ToName = table.Column<string>(maxLength: 500, nullable: true),
                    ReplyTo = table.Column<string>(maxLength: 500, nullable: true),
                    ReplyToName = table.Column<string>(maxLength: 500, nullable: true),
                    CC = table.Column<string>(maxLength: 500, nullable: true),
                    Bcc = table.Column<string>(maxLength: 500, nullable: true),
                    Subject = table.Column<string>(maxLength: 1000, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    AttachmentFilePath = table.Column<string>(nullable: true),
                    AttachmentFileName = table.Column<string>(nullable: true),
                    AttachedDownloadId = table.Column<int>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    DontSendBeforeDateUtc = table.Column<DateTime>(nullable: true),
                    SentTries = table.Column<int>(nullable: false),
                    SentOnUtc = table.Column<DateTime>(nullable: true),
                    EmailAccountId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEmail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueuedEmail_EmailAccount_EmailAccountId",
                        column: x => x.EmailAccountId,
                        principalTable: "EmailAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocaleStringResource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LanguageId = table.Column<int>(nullable: false),
                    ResourceName = table.Column<string>(maxLength: 200, nullable: false),
                    ResourceValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocaleStringResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocaleStringResource_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocalizedProperty",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    LocaleKeyGroup = table.Column<string>(maxLength: 400, nullable: false),
                    LocaleKey = table.Column<string>(maxLength: 400, nullable: false),
                    LocaleValue = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocalizedProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocalizedProperty_Language_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PictureBinary",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BinaryData = table.Column<byte[]>(nullable: true),
                    PictureId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureBinary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PictureBinary_Picture_PictureId",
                        column: x => x.PictureId,
                        principalTable: "Picture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityLogTypeId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: true),
                    EntityName = table.Column<string>(maxLength: 400, nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityLog_ActivityLogType_ActivityLogTypeId",
                        column: x => x.ActivityLogTypeId,
                        principalTable: "ActivityLogType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityLog_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAuthenticationRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    ExternalIdentifier = table.Column<string>(nullable: true),
                    ExternalDisplayIdentifier = table.Column<string>(nullable: true),
                    OAuthToken = table.Column<string>(nullable: true),
                    OAuthAccessToken = table.Column<string>(nullable: true),
                    ProviderSystemName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAuthenticationRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAuthenticationRecord_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogLevelId = table.Column<int>(nullable: false),
                    ShortMessage = table.Column<string>(nullable: false),
                    FullMessage = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 200, nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    PageUrl = table.Column<string>(nullable: true),
                    ReferrerUrl = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPassword",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    PasswordFormatId = table.Column<int>(nullable: false),
                    PasswordSalt = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPassword", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPassword_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAttributeValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserAttributeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 400, nullable: false),
                    IsPreSelected = table.Column<bool>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAttributeValue_UserAttribute_UserAttributeId",
                        column: x => x.UserAttributeId,
                        principalTable: "UserAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AclRecord",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    EntityName = table.Column<string>(maxLength: 400, nullable: false),
                    UserRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AclRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AclRecord_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRecord_Role_Mapping",
                columns: table => new
                {
                    PermissionRecord_Id = table.Column<int>(nullable: false),
                    UserRole_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRecord_Role_Mapping", x => new { x.PermissionRecord_Id, x.UserRole_Id });
                    table.ForeignKey(
                        name: "FK_PermissionRecord_Role_Mapping_PermissionRecord_PermissionRecord_Id",
                        column: x => x.PermissionRecord_Id,
                        principalTable: "PermissionRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRecord_Role_Mapping_UserRole_UserRole_Id",
                        column: x => x.UserRole_Id,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_UserRole_Mapping",
                columns: table => new
                {
                    User_Id = table.Column<int>(nullable: false),
                    UserRole_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_UserRole_Mapping", x => new { x.User_Id, x.UserRole_Id });
                    table.ForeignKey(
                        name: "FK_User_UserRole_Mapping_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_UserRole_Mapping_UserRole_UserRole_Id",
                        column: x => x.UserRole_Id,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    CountryId = table.Column<int>(nullable: true),
                    StateProvinceId = table.Column<int>(nullable: true),
                    County = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    ZipPostalCode = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    FaxNumber = table.Column<string>(nullable: true),
                    CustomAttributes = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_StateProvince_StateProvinceId",
                        column: x => x.StateProvinceId,
                        principalTable: "StateProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    User_Id = table.Column<int>(nullable: false),
                    Address_Id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => new { x.User_Id, x.Address_Id });
                    table.ForeignKey(
                        name: "FK_UserAddresses_Address_Address_Id",
                        column: x => x.Address_Id,
                        principalTable: "Address",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAddresses_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AclRecord_UserRoleId",
                table: "AclRecord",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_ActivityLogTypeId",
                table: "ActivityLog",
                column: "ActivityLogTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_UserId",
                table: "ActivityLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateProvinceId",
                table: "Address",
                column: "StateProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AddressAttributeValue_AddressAttributeId",
                table: "AddressAttributeValue",
                column: "AddressAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAuthenticationRecord_UserId",
                table: "ExternalAuthenticationRecord",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocaleStringResource_LanguageId",
                table: "LocaleStringResource",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalizedProperty_LanguageId",
                table: "LocalizedProperty",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRecord_Role_Mapping_UserRole_Id",
                table: "PermissionRecord_Role_Mapping",
                column: "UserRole_Id");

            migrationBuilder.CreateIndex(
                name: "IX_PictureBinary_PictureId",
                table: "PictureBinary",
                column: "PictureId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueuedEmail_EmailAccountId",
                table: "QueuedEmail",
                column: "EmailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StateProvince_CountryId",
                table: "StateProvince",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserRole_Mapping_UserRole_Id",
                table: "User_UserRole_Mapping",
                column: "UserRole_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_Address_Id",
                table: "UserAddresses",
                column: "Address_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAttributeValue_UserAttributeId",
                table: "UserAttributeValue",
                column: "UserAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPassword_UserId",
                table: "UserPassword",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AclRecord");

            migrationBuilder.DropTable(
                name: "ActivityLog");

            migrationBuilder.DropTable(
                name: "AddressAttributeValue");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropTable(
                name: "Download");

            migrationBuilder.DropTable(
                name: "ExternalAuthenticationRecord");

            migrationBuilder.DropTable(
                name: "GenericAttribute");

            migrationBuilder.DropTable(
                name: "LocaleStringResource");

            migrationBuilder.DropTable(
                name: "LocalizedProperty");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "MessageTemplate");

            migrationBuilder.DropTable(
                name: "NewsLetterSubscription");

            migrationBuilder.DropTable(
                name: "PermissionRecord_Role_Mapping");

            migrationBuilder.DropTable(
                name: "PictureBinary");

            migrationBuilder.DropTable(
                name: "QueuedEmail");

            migrationBuilder.DropTable(
                name: "ScheduleTask");

            migrationBuilder.DropTable(
                name: "SearchTerm");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "UrlRecord");

            migrationBuilder.DropTable(
                name: "User_UserRole_Mapping");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserAttributeValue");

            migrationBuilder.DropTable(
                name: "UserPassword");

            migrationBuilder.DropTable(
                name: "ActivityLogType");

            migrationBuilder.DropTable(
                name: "AddressAttribute");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "PermissionRecord");

            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "EmailAccount");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "UserAttribute");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "StateProvince");

            migrationBuilder.DropTable(
                name: "Country");
        }
    }
}
