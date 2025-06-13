using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.API.Migrations
{
    public partial class AddFK_Category_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Nếu constraint cũ đã tồn tại, ta drop nó trước
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            // Thêm lại FK với delete behavior NoAction (không cascade)
            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Trong phương thức Down, hoàn tác lại thay đổi: drop FK hiện tại và thêm lại với Cascade (nếu mặc định trước đó là Cascade)
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}