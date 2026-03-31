using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace logindemo.Migrations
{
    /// <inheritdoc />
    public partial class Allincluded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PoliceStationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SessionToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoliceStations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Incharge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliceStations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PoliceStationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PoliceStationName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Issue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PoliceStationId = table.Column<int>(type: "int", nullable: true),
                    ReporterName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssueType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CompletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovalDocuments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_PoliceStations_PoliceStationId",
                        column: x => x.PoliceStationId,
                        principalTable: "PoliceStations",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "PoliceStations",
                columns: new[] { "Id", "ContactNumber", "CreatedAt", "Email", "IPAddress", "Incharge", "Location", "PasswordHash", "Region", "StationCode", "StationName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001001", "FOOD CELL PS KARAIKAL" },
                    { 2, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001002", "KOTTUCHERRY" },
                    { 3, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001003", "TOWN" },
                    { 4, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001004", "NEDUNGADU" },
                    { 5, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001005", "NERAVY" },
                    { 6, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001006", "THIRUNALLAR" },
                    { 7, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001007", "T.R.PATTINAM" },
                    { 8, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001008", "WOMEN PS" },
                    { 9, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001009", "PCR CELL PS KARAIKAL" },
                    { 10, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001010", "TRAFFIC NORTH" },
                    { 11, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001011", "EXCISE PS KARAIKAL" },
                    { 12, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001012", "COASTAL PS KARAIKAL" },
                    { 13, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26001013", "TRAFFIC SOUTH" },
                    { 14, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002001", "VAC PS (VIGILANCE & ANTI CORRUPTION)" },
                    { 15, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002002", "CID PS" },
                    { 16, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002003", "DHANVANTRI NAGAR (D. NAGAR)" },
                    { 17, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002004", "REDDIARPALAYAM" },
                    { 18, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002005", "MUDALIARPET" },
                    { 19, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002006", "METTUPALAYAM" },
                    { 20, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002007", "VILLIANUR" },
                    { 21, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002008", "WOMEN PS VILLIANOUR" },
                    { 22, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002009", "MANGALAM" },
                    { 23, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002010", "FOOD CELL PS PUDUCHERRY" },
                    { 24, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002011", "MAHE PS" },
                    { 25, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002012", "PALLOOR PS" },
                    { 26, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002013", "YANAM PS" },
                    { 27, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002014", "PCR CELL PS PUDUCHERRY" },
                    { 28, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002015", "PCR CELL PS YANAM" },
                    { 29, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002016", "GRAND BAZAR" },
                    { 30, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002017", "KALAPET" },
                    { 31, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002018", "MUTHIALPET" },
                    { 32, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002019", "LAWSPET" },
                    { 33, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002020", "ODIANSALAI" },
                    { 34, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002021", "ORLEANPET" },
                    { 35, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002022", "WOMEN PS" },
                    { 36, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002023", "TRAFFIC EAST" },
                    { 37, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002024", "TRAFFIC WEST" },
                    { 38, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002025", "ARIANKUPPAM" },
                    { 39, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002026", "SEDARAPET" },
                    { 40, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002027", "BAHOUR" },
                    { 41, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002028", "KIRUMAMPAKKAM" },
                    { 42, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002029", "KATTERIKUPPAM" },
                    { 43, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002030", "NETTAPAKKAM" },
                    { 44, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002031", "THAVALAKUPPAM" },
                    { 45, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002032", "THIRUBUVANAI" },
                    { 46, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002033", "THIRUKANUR" },
                    { 47, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002034", "EXCISE PS MAHE" },
                    { 48, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002035", "EXCISE PS YANAM" },
                    { 49, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002036", "TRAFFIC SOUTH" },
                    { 50, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002037", "EXCISE PS PUDUCHERRY" },
                    { 51, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002038", "COASTAL PS PUDUCHERRY" },
                    { 52, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002039", "COASTAL PS MAHE" },
                    { 53, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002040", "COASTAL PS YANAM" },
                    { 54, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002041", "TRAFFIC NORTH" },
                    { 55, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002042", "PS ECONOMIC OFFENSES WING" },
                    { 56, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002043", "PS CYBER CRIME CELL" },
                    { 57, null, new DateTime(2025, 6, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), null, null, null, null, "", null, "26002044", "TRAFFIC YANAM POLICE STATION" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PoliceStationId",
                table: "Tickets",
                column: "PoliceStationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PoliceStations");
        }
    }
}
