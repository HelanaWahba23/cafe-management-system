using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_CustomerName_NotBlank",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_CustomerPhone_NotBlank",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "Orders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Orders",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Creator",
                table: "Orders",
                sql: "([Origin] = N'Customer' AND [CreatedByUserId] IS NULL) OR ([Origin] = N'Counter' AND [CreatedByUserId] IS NOT NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_CustomerName_NotBlank",
                table: "Orders",
                sql: "([Origin] = N'Counter' AND [CustomerName] IS NULL) OR ([CustomerName] IS NOT NULL AND LEN(LTRIM(RTRIM([CustomerName]))) > 0)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_CustomerPhone_NotBlank",
                table: "Orders",
                sql: "([Origin] = N'Counter' AND [CustomerPhone] IS NULL) OR ([CustomerPhone] IS NOT NULL AND LEN(LTRIM(RTRIM([CustomerPhone]))) > 0)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Origin",
                table: "Orders",
                sql: "[Origin] IN (N'Customer', N'Counter')");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedByUserId",
                table: "Orders",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Creator",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_CustomerName_NotBlank",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_CustomerPhone_NotBlank",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Origin",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerPhone",
                table: "Orders",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(120)",
                oldMaxLength: 120,
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_CustomerName_NotBlank",
                table: "Orders",
                sql: "LEN(LTRIM(RTRIM([CustomerName]))) > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_CustomerPhone_NotBlank",
                table: "Orders",
                sql: "LEN(LTRIM(RTRIM([CustomerPhone]))) > 0");
        }
    }
}
