using Microsoft.EntityFrameworkCore.Migrations;

namespace Tedd.ShortUrl.Migrations
{
    public partial class IndexKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Urls_Key",
                table: "Urls",
                column: "Key",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Urls_Key",
                table: "Urls");
        }
    }
}
