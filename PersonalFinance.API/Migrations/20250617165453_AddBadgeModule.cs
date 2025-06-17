using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinance.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBadgeModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwardedDate",
                table: "Badges");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Badges",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Badges",
                newName: "AwardedAt");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Badges",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Badges",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Badges");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Badges");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Badges",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "AwardedAt",
                table: "Badges",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "AwardedDate",
                table: "Badges",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
