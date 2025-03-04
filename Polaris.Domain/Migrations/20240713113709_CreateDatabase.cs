using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace Polaris.Domain.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "application",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APPLICATION", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "varchar(256)", unicode: false, maxLength: 256, nullable: false),
                    language = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    userId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    applicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MEMBER", x => x.id);
                    table.ForeignKey(
                        name: "FK_MEMBER_APPLICATION",
                        column: x => x.applicationId,
                        principalTable: "application",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_MEMBER_USER",
                        column: x => x.userId,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authentication",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    memberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    refreshToken = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    codeAttempt = table.Column<int>(type: "int", unicode: false, maxLength: 10, nullable: true),
                    type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    codeExpiration = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUTHENTICATION", x => x.id);
                    table.ForeignKey(
                        name: "FK_AUTHENTICATION_MEMBER",
                        column: x => x.memberId,
                        principalTable: "member",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_AUTHENTICATION_MEMBER",
                table: "authentication",
                column: "memberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_member_applicationId",
                table: "member",
                column: "applicationId");

            migrationBuilder.CreateIndex(
                name: "IX_member_userId",
                table: "member",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_USER_EMAIL",
                table: "user",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authentication");

            migrationBuilder.DropTable(
                name: "member");

            migrationBuilder.DropTable(
                name: "application");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
