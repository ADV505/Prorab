using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCrewForeman : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crews_Workers_ForemanId",
                table: "Crews");

            migrationBuilder.DropIndex(
                name: "IX_Crews_ForemanId",
                table: "Crews");

            migrationBuilder.DropColumn(
                name: "ForemanId",
                table: "Crews");

            migrationBuilder.CreateIndex(
                name: "IX_Crews_ForemanWorkerId",
                table: "Crews",
                column: "ForemanWorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Crews_Workers_ForemanWorkerId",
                table: "Crews",
                column: "ForemanWorkerId",
                principalTable: "Workers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crews_Workers_ForemanWorkerId",
                table: "Crews");

            migrationBuilder.DropIndex(
                name: "IX_Crews_ForemanWorkerId",
                table: "Crews");

            migrationBuilder.AddColumn<int>(
                name: "ForemanId",
                table: "Crews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Crews_ForemanId",
                table: "Crews",
                column: "ForemanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Crews_Workers_ForemanId",
                table: "Crews",
                column: "ForemanId",
                principalTable: "Workers",
                principalColumn: "Id");
        }
    }
}
