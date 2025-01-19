using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.Domain.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class CreateTableRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refreshToken",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    authenticationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    used = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKEN", x => x.id);
                    table.ForeignKey(
                        name: "FK_REFRESH_TOKEN_AUTHENTICATION",
                        column: x => x.authenticationId,
                        principalTable: "authentication",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_AuthenticationId",
                table: "refreshToken",
                column: "authenticationId");

            migrationBuilder.Sql(@"
INSERT INTO refreshToken ([Id], [AuthenticationId], [Used])
SELECT [refreshToken], [id], 0
FROM [authentication]
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refreshToken");
        }
    }
}
