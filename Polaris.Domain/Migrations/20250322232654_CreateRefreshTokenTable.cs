using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Polaris.Domain.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class CreateRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    AuthenticationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Expiration = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REFRESH_TOKEN", x => x.Id);
                    table.ForeignKey(
                        name: "FK_REFRESH_TOKEN_AUTHENTICATION",
                        column: x => x.AuthenticationId,
                        principalTable: "authentication",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_refreshToken_AuthenticationId",
                table: "refreshToken",
                column: "AuthenticationId");

            migrationBuilder.Sql(@"
INSERT INTO [refreshToken] (id,authenticationId,expiration,token)
SELECT NEWID(), [Id], GETDATE() + 30, [refreshToken]
FROM [authentication] WHERE refreshToken is not null
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
