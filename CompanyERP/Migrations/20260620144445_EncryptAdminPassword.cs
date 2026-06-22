using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyERP.Migrations
{
    /// <inheritdoc />
    public partial class EncryptAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPPRs2UMr8NwSnFRJWurFwLjAngWRxzyYZIVc8nd3n2cgP1Kxs38DoOhWwateb3ABg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "Admin123!");
        }
    }
}
