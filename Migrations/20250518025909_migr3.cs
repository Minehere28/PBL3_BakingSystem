using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PBL3.Migrations
{
    /// <inheritdoc />
    public partial class migr3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Users_userSdt",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_userSdt",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "userSdt",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Sdt",
                table: "BankAccounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_Sdt",
                table: "BankAccounts",
                column: "Sdt");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Users_Sdt",
                table: "BankAccounts",
                column: "Sdt",
                principalTable: "Users",
                principalColumn: "Sdt",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Users_Sdt",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_Sdt",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "Sdt",
                table: "BankAccounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "userSdt",
                table: "BankAccounts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_userSdt",
                table: "BankAccounts",
                column: "userSdt");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Users_userSdt",
                table: "BankAccounts",
                column: "userSdt",
                principalTable: "Users",
                principalColumn: "Sdt",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
