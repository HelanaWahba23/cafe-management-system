using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reference = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CapabilityHash = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: false),
                    IntentHash = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    TableId = table.Column<int>(type: "int", nullable: true),
                    TableLabel = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CustomerPhone = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", unicode: false, fixedLength: true, maxLength: 3, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CK_Orders_Currency", "[Currency] = 'EGP'");
                    table.CheckConstraint("CK_Orders_CustomerName_NotBlank", "LEN(LTRIM(RTRIM([CustomerName]))) > 0");
                    table.CheckConstraint("CK_Orders_CustomerPhone_NotBlank", "LEN(LTRIM(RTRIM([CustomerPhone]))) > 0");
                    table.CheckConstraint("CK_Orders_RejectionReason", "([Status] = N'Rejected' AND [RejectionReason] IS NOT NULL AND LEN(LTRIM(RTRIM([RejectionReason]))) > 0) OR ([Status] <> N'Rejected' AND [RejectionReason] IS NULL)");
                    table.CheckConstraint("CK_Orders_Status", "[Status] IN (N'Submitted', N'Preparing', N'Ready', N'Completed', N'Rejected')");
                    table.CheckConstraint("CK_Orders_TableContext", "([Type] = N'DineIn' AND [TableId] IS NOT NULL AND [TableLabel] IS NOT NULL AND LEN(LTRIM(RTRIM([TableLabel]))) > 0) OR ([Type] = N'Pickup' AND [TableId] IS NULL AND [TableLabel] IS NULL)");
                    table.CheckConstraint("CK_Orders_Total_Positive", "[Total] > 0");
                    table.CheckConstraint("CK_Orders_Type", "[Type] IN (N'DineIn', N'Pickup')");
                    table.ForeignKey(
                        name: "FK_Orders_CafeTables_TableId",
                        column: x => x.TableId,
                        principalTable: "CafeTables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLines", x => x.Id);
                    table.CheckConstraint("CK_OrderLines_LineTotal_Arithmetic", "CONVERT(decimal(29,2), [LineTotal]) = CONVERT(decimal(18,2), [UnitPrice]) * CONVERT(decimal(10,0), [Quantity])");
                    table.CheckConstraint("CK_OrderLines_LineTotal_Positive", "[LineTotal] > 0");
                    table.CheckConstraint("CK_OrderLines_ProductName_NotBlank", "LEN(LTRIM(RTRIM([ProductName]))) > 0");
                    table.CheckConstraint("CK_OrderLines_Quantity_Positive", "[Quantity] > 0");
                    table.CheckConstraint("CK_OrderLines_UnitPrice_Positive", "[UnitPrice] > 0");
                    table.ForeignKey(
                        name: "FK_OrderLines_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderLines_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_OrderId_ProductId",
                table: "OrderLines",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderLines_ProductId",
                table: "OrderLines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CapabilityHash",
                table: "Orders",
                column: "CapabilityHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Reference",
                table: "Orders",
                column: "Reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_CreatedAtUtc",
                table: "Orders",
                columns: new[] { "Status", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderLines");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
