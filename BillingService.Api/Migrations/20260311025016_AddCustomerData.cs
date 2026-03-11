using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CoveredAmount",
                table: "PremiumSchedules",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PolicyNumber",
                table: "PremiumSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Policies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Premium",
                table: "Policies",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoveredAmount",
                table: "PremiumSchedules");

            migrationBuilder.DropColumn(
                name: "PolicyNumber",
                table: "PremiumSchedules");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "Premium",
                table: "Policies");
        }
    }
}
