using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockMarket.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trades_BuyOrderId",
                table: "Trades",
                column: "BuyOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_SellOrderId",
                table: "Trades",
                column: "SellOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Orders_BuyOrderId",
                table: "Trades",
                column: "BuyOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Trades_Orders_SellOrderId",
                table: "Trades",
                column: "SellOrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Orders_BuyOrderId",
                table: "Trades");

            migrationBuilder.DropForeignKey(
                name: "FK_Trades_Orders_SellOrderId",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_BuyOrderId",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_SellOrderId",
                table: "Trades");
        }
    }
}
