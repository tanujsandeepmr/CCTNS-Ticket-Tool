using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace logindemo.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfPathToTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 3,
                column: "StationName",
                value: "TOWN KARAIKAL");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 8,
                column: "StationName",
                value: "WOMEN PS KARAIKAL");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 10,
                column: "StationName",
                value: "TRAFFIC NORTH KARAIKAL");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 13,
                column: "StationName",
                value: "TRAFFIC SOUTH KARAIKAL");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 15,
                column: "StationName",
                value: "CBCID PS");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 21,
                column: "StationName",
                value: "WOMEN PS VILLIANUR");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 36,
                column: "StationName",
                value: "TRAFFIC EAST PUDUCHERRY");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 37,
                column: "StationName",
                value: "TRAFFIC WEST PUDUCHERRY");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 49,
                column: "StationName",
                value: "TRAFFIC SOUTH PUDUCHERRY");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 54,
                column: "StationName",
                value: "TRAFFIC NORTH PUDUCHERRY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "Tickets");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 3,
                column: "StationName",
                value: "TOWN");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 8,
                column: "StationName",
                value: "WOMEN PS");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 10,
                column: "StationName",
                value: "TRAFFIC NORTH");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 13,
                column: "StationName",
                value: "TRAFFIC SOUTH");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 15,
                column: "StationName",
                value: "CID PS");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 21,
                column: "StationName",
                value: "WOMEN PS VILLIANOUR");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 36,
                column: "StationName",
                value: "TRAFFIC EAST");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 37,
                column: "StationName",
                value: "TRAFFIC WEST");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 49,
                column: "StationName",
                value: "TRAFFIC SOUTH");

            migrationBuilder.UpdateData(
                table: "PoliceStations",
                keyColumn: "Id",
                keyValue: 54,
                column: "StationName",
                value: "TRAFFIC NORTH");
        }
    }
}
