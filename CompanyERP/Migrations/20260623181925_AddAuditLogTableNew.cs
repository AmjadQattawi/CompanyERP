using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyERP.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogTableNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOE4swqmYeHG+hn0IBwEBCHFJw1a1yD8ekKoFccpkE6XiUwuZgaxpznKRKBIbQlCQg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 99,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIpoYD3XsavuPeJX7x0Loq5ZL5OT2k8FE0xqvahed1wGQXaAlNkxbD5n+ZxI0yRfDw==");
        }
    }
}
