using Microsoft.EntityFrameworkCore.Migrations;

namespace Tedd.ShortUrl.Migrations
{
    public partial class AuthTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanGet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthTokens", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_AuthToken",
                table: "AuthTokens",
                column: "AuthToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthTokens");
        }
    }
}
