using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectData.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assistant",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", nullable: false),
                    UserStatus = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "IsValidated"),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    LoginTimes = table.Column<string>(type: "Integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assistant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsStart = table.Column<int>(maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(type: "varchar(100)", nullable: true),
                    Password = table.Column<string>(type: "varchar(100)", nullable: true),
                    Profile = table.Column<string>(type: "varchar(100)", nullable: true),
                    Gender = table.Column<string>(type: "char(1)", nullable: true),
                    Age = table.Column<string>(type: "varchar(2)", nullable: true),
                    Hearing = table.Column<string>(type: "varchar(100)", nullable: true),
                    AssistantId = table.Column<int>(nullable: true),
                    JumpForFunScore = table.Column<int>(nullable: true),
                    CubeHubScore = table.Column<int>(nullable: true),
                    UserStatus = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "IsValidated"),
                    Email = table.Column<string>(type: "varchar(100)", nullable: false),
                    LoginTimes = table.Column<string>(type: "Integer", nullable: false, defaultValue: 0),
                    DeviceId = table.Column<string>(type: "varchar(100)", nullable: false),
                    ValidationCode = table.Column<string>(type: "varchar(10)", nullable: false),
                    ExpiredTime = table.Column<string>(type: "Integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Assistant_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "Assistant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "English",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    TypeId = table.Column<int>(nullable: true),
                    Detail = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_English", x => x.Id);
                    table.ForeignKey(
                        name: "FK_English_Type_TypeId",
                        column: x => x.TypeId,
                        principalTable: "Type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Progress",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EnglishId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    FinishDate = table.Column<DateTime>(type: "date", nullable: false),
                    Grade = table.Column<int>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Progress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Progress_English_EnglishId",
                        column: x => x.EnglishId,
                        principalTable: "English",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Progress_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    Id = table.Column<int>(maxLength: 10, nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    EnglishId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedule_English_EnglishId",
                        column: x => x.EnglishId,
                        principalTable: "English",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Schedule_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_English_TypeId",
                table: "English",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Progress_EnglishId",
                table: "Progress",
                column: "EnglishId");

            migrationBuilder.CreateIndex(
                name: "IX_Progress_UserId",
                table: "Progress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_EnglishId",
                table: "Schedule",
                column: "EnglishId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_UserId",
                table: "Schedule",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_AssistantId",
                table: "User",
                column: "AssistantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropTable(
                name: "Progress");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "English");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.DropTable(
                name: "Assistant");
        }
    }
}
