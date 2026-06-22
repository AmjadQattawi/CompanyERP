using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyERP.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminAndMainBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Project",
                type: "DECIMAL(18, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18,2)");

            migrationBuilder.InsertData(
                table: "Branch",
                columns: new[] { "Id", "BranchCode", "BranchName" },
                values: new object[] { 99, "HQ01", "Main Headquarter" });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "BranchId", "Email", "FullName", "PasswordHash", "Role", "Salary" },
                values: new object[] { 99, 99, "admin@company.com", "System Admin", "Admin123!", 0, 2500m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Branch",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget",
                table: "Project",
                type: "DECIMAL(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(18, 2)");
        }
    }
}
