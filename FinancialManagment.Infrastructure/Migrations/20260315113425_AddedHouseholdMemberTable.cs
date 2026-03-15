using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialManagment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedHouseholdMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_AspNetUsers_ApplicationUserId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_AspNetUsers_ApplicationUserId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_ApplicationUserId_Date_IncomeCategoryId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ApplicationUserId_Date_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "HouseholdMemberId",
                table: "Incomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HouseholdMemberId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HouseholdMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdMembers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_HouseholdMemberId_Date_IncomeCategoryId",
                table: "Incomes",
                columns: new[] { "HouseholdMemberId", "Date", "IncomeCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_HouseholdMemberId_Date_ExpenseCategoryId",
                table: "Expenses",
                columns: new[] { "HouseholdMemberId", "Date", "ExpenseCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMembers_ApplicationUserId_Nickname",
                table: "HouseholdMembers",
                columns: new[] { "ApplicationUserId", "Nickname" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_HouseholdMembers_HouseholdMemberId",
                table: "Expenses",
                column: "HouseholdMemberId",
                principalTable: "HouseholdMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_HouseholdMembers_HouseholdMemberId",
                table: "Incomes",
                column: "HouseholdMemberId",
                principalTable: "HouseholdMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_HouseholdMembers_HouseholdMemberId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Incomes_HouseholdMembers_HouseholdMemberId",
                table: "Incomes");

            migrationBuilder.DropTable(
                name: "HouseholdMembers");

            migrationBuilder.DropIndex(
                name: "IX_Incomes_HouseholdMemberId_Date_IncomeCategoryId",
                table: "Incomes");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_HouseholdMemberId_Date_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "HouseholdMemberId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "HouseholdMemberId",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Incomes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Expenses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_ApplicationUserId_Date_IncomeCategoryId",
                table: "Incomes",
                columns: new[] { "ApplicationUserId", "Date", "IncomeCategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ApplicationUserId_Date_ExpenseCategoryId",
                table: "Expenses",
                columns: new[] { "ApplicationUserId", "Date", "ExpenseCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_AspNetUsers_ApplicationUserId",
                table: "Expenses",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Incomes_AspNetUsers_ApplicationUserId",
                table: "Incomes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
