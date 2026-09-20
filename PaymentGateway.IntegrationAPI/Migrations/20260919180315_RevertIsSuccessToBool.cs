using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentGateway.IntegrationAPI.Migrations
{
    /// <inheritdoc />
    public partial class RevertIsSuccessToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSuccess",
                table: "PaymentTransactions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IsSuccess",
                table: "PaymentTransactions",
                type: "nvarchar(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
