using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHouseholdMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HouseholdMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HouseholdMembers");
        }
    }
}
