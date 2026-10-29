using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeManagement.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PaymentRevision",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", unicode: false, nullable: false),
                    IntegrationId = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsBlocking = table.Column<bool>(type: "bit", nullable: false),
                    ProviderOrderId = table.Column<long>(type: "bigint", nullable: true),
                    ProviderIntentionId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    ProtectedClientSecret = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.CheckConstraint("CK_PaymentAttempts_Amount", "[Amount] > 0 AND [Currency] = 'EGP' AND [IntegrationId] > 0");
                    table.CheckConstraint("CK_PaymentAttempts_ProviderOrder", "[ProviderOrderId] IS NULL OR [ProviderOrderId] > 0");
                    table.CheckConstraint("CK_PaymentAttempts_State", "([State] IN (N'Creating', N'Pending', N'Uncertain') AND [IsBlocking] = 1) OR ([State] IN (N'Failed', N'Succeeded') AND [IsBlocking] = 0)");
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSettlements",
                columns: table => new
                {
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", unicode: false, nullable: false),
                    Method = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    RecordedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProviderTransactionId = table.Column<long>(type: "bigint", nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSettlements", x => x.OrderId);
                    table.CheckConstraint("CK_PaymentSettlements_Amount", "[Amount] > 0 AND [Currency] = 'EGP'");
                    table.CheckConstraint("CK_PaymentSettlements_Method", "([Method] = N'Cash' AND [RecordedByUserId] IS NOT NULL AND [ProviderTransactionId] IS NULL) OR ([Method] = N'PaymobSandbox' AND [RecordedByUserId] IS NULL AND [ProviderTransactionId] IS NOT NULL AND [ProviderTransactionId] > 0)");
                    table.ForeignKey(
                        name: "FK_PaymentSettlements_AspNetUsers_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentSettlements_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_OrderId",
                table: "PaymentAttempts",
                column: "OrderId",
                unique: true,
                filter: "[IsBlocking] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_ProviderOrderId",
                table: "PaymentAttempts",
                column: "ProviderOrderId",
                unique: true,
                filter: "[ProviderOrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSettlements_ProviderTransactionId",
                table: "PaymentSettlements",
                column: "ProviderTransactionId",
                unique: true,
                filter: "[ProviderTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSettlements_RecordedAtUtc",
                table: "PaymentSettlements",
                column: "RecordedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSettlements_RecordedByUserId",
                table: "PaymentSettlements",
                column: "RecordedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PaymentSettlements");

            migrationBuilder.DropColumn(
                name: "PaymentRevision",
                table: "Orders");
        }
    }
}
