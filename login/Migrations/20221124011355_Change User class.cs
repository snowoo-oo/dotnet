using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace login.Migrations
{
    public partial class ChangeUserclass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "Group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "Group",
                table: "Users",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
