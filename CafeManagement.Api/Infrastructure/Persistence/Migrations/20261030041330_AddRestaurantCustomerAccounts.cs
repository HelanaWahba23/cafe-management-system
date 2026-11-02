using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantCustomerAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_TableContext",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Type",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "CustomerUserId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "MenuCategories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "CustomerProfiles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_CustomerProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerUserId",
                table: "Orders",
                column: "CustomerUserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_DeliveryAddress",
                table: "Orders",
                sql: "([Type] = N'Delivery' AND [DeliveryAddress] IS NOT NULL AND LEN(LTRIM(RTRIM([DeliveryAddress]))) BETWEEN 8 AND 500) OR ([Type] <> N'Delivery' AND [DeliveryAddress] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders",
                sql: "[Status] IN (N'Submitted', N'Accepted', N'Preparing', N'Ready', N'OutForDelivery', N'Completed', N'Rejected')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_TableContext",
                table: "Orders",
                sql: "([Type] = N'DineIn' AND [TableId] IS NOT NULL AND [TableLabel] IS NOT NULL AND LEN(LTRIM(RTRIM([TableLabel]))) > 0) OR ([Type] IN (N'Pickup', N'Delivery') AND [TableId] IS NULL AND [TableLabel] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Type",
                table: "Orders",
                sql: "[Type] IN (N'DineIn', N'Pickup', N'Delivery')");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerUserId",
                table: "Orders",
                column: "CustomerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerUserId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "CustomerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerUserId",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_DeliveryAddress",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_TableContext",
                table: "Orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Type",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsVisible",
                table: "MenuCategories");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders",
                sql: "[Status] IN (N'Submitted', N'Preparing', N'Ready', N'Completed', N'Rejected')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_TableContext",
                table: "Orders",
                sql: "([Type] = N'DineIn' AND [TableId] IS NOT NULL AND [TableLabel] IS NOT NULL AND LEN(LTRIM(RTRIM([TableLabel]))) > 0) OR ([Type] = N'Pickup' AND [TableId] IS NULL AND [TableLabel] IS NULL)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Type",
                table: "Orders",
                sql: "[Type] IN (N'DineIn', N'Pickup')");
        }
    }
}
