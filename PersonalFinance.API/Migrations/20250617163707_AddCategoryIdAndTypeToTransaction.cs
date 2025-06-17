using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.API.Migrations
{
    public partial class AddCategoryIdAndTypeToTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm cột CategoryId cho bảng Transactions, cho phép null nếu không có giá trị.
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Transactions",
                type: "int",
                nullable: true);  // Cho phép null thay vì false

            // Thêm cột Type cho bảng Transactions. Giá trị mặc định có thể là chuỗi rỗng hoặc "expense" tùy theo nghiệp vụ.
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // Tạo khóa ngoại nếu cột CategoryId có giá trị (không thực thi nếu giá trị là null).
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);  // Hoặc ReferentialAction.SetNull nếu muốn
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Categories_CategoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");
        }
    }
}