using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    public partial class FixedForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Tests_TestId",
                table: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Histories_TestId",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "Histories");

            migrationBuilder.AddColumn<int>(
                name: "HistoryId",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "HashedPassword",
                value: "AAfzFHfMOJjf16urY1NxWhnuEU5+56N1NKNsez2HrqMhjf9PE7zRyTFTLYOAWS6dog==");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_HistoryId",
                table: "Tests",
                column: "HistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Histories_PatientId",
                table: "Histories",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Patients_PatientId",
                table: "Histories",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Histories_HistoryId",
                table: "Tests",
                column: "HistoryId",
                principalTable: "Histories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Histories_Patients_PatientId",
                table: "Histories");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Histories_HistoryId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_HistoryId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Histories_PatientId",
                table: "Histories");

            migrationBuilder.DropColumn(
                name: "HistoryId",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "TestId",
                table: "Histories",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Histories_Tests_TestId",
                table: "Histories",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
