using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelGuard.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddQueryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FuelTransactions_TankId",
                table: "FuelTransactions");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_TankId_OccurredAt",
                table: "FuelTransactions",
                columns: new[] { "TankId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_DailyFuelSnapshots_Date",
                table: "DailyFuelSnapshots",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_Name",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyDetections_DetectedAt",
                table: "AnomalyDetections",
                column: "DetectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AnomalyDetections_TankId_SnapshotDate",
                table: "AnomalyDetections",
                columns: new[] { "TankId", "SnapshotDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FuelTransactions_TankId_OccurredAt",
                table: "FuelTransactions");

            migrationBuilder.DropIndex(
                name: "IX_DailyFuelSnapshots_Date",
                table: "DailyFuelSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_Companies_Name",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_AnomalyDetections_DetectedAt",
                table: "AnomalyDetections");

            migrationBuilder.DropIndex(
                name: "IX_AnomalyDetections_TankId_SnapshotDate",
                table: "AnomalyDetections");

            migrationBuilder.CreateIndex(
                name: "IX_FuelTransactions_TankId",
                table: "FuelTransactions",
                column: "TankId");
        }
    }
}
