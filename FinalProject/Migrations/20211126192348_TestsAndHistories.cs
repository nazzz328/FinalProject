using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class TestsAndHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Haemoglobin = table.Column<double>(type: "float", nullable: false),
                    Erythrocytes = table.Column<double>(type: "float", nullable: false),
                    Leukocytes = table.Column<double>(type: "float", nullable: false),
                    Ultrasound = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ECG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HepatitisB = table.Column<bool>(type: "bit", nullable: false),
                    HepatitisC = table.Column<bool>(type: "bit", nullable: false),
                    AIDS = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Complaints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anamnesis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inspection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conclusion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Histories_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashedPassword",
                value: "AKLyAEDIw2JXPM66U2r8kCQA61GqTxToU6nYJdNj5T1gBTIze98stm/ixUNpRrJIIA==");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_TestId",
                table: "Histories",
                column: "TestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashedPassword",
                value: "AJk+ZMJz/4wE4vJvHODnhYhoW4v+xyDbaRvOyh/PjQlEhmML4bvIAJTNwI599I+8GQ==");
        }
    }
}
