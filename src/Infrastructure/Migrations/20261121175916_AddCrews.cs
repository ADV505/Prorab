using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCrews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForemanName",
                table: "Crews");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Crews");

            migrationBuilder.AddColumn<int>(
                name: "ForemanId",
                table: "Crews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ForemanWorkerId",
                table: "Crews",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CrewSites",
                columns: table => new
                {
                    CrewId = table.Column<int>(type: "INTEGER", nullable: false),
                    SiteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewSites", x => new { x.CrewId, x.SiteId });
                    table.ForeignKey(
                        name: "FK_CrewSites_Crews_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrewSites_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrewWorkers",
                columns: table => new
                {
                    CrewId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrewWorkers", x => new { x.CrewId, x.WorkerId });
                    table.ForeignKey(
                        name: "FK_CrewWorkers_Crews_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CrewWorkers_Workers_WorkerId",
                        column: x => x.WorkerId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Crews_ForemanId",
                table: "Crews",
                column: "ForemanId");

            migrationBuilder.CreateIndex(
                name: "IX_CrewSites_SiteId",
                table: "CrewSites",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CrewWorkers_WorkerId",
                table: "CrewWorkers",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Crews_Workers_ForemanId",
                table: "Crews",
                column: "ForemanId",
                principalTable: "Workers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Crews_Workers_ForemanId",
                table: "Crews");

            migrationBuilder.DropTable(
                name: "CrewSites");

            migrationBuilder.DropTable(
                name: "CrewWorkers");

            migrationBuilder.DropIndex(
                name: "IX_Crews_ForemanId",
                table: "Crews");

            migrationBuilder.DropColumn(
                name: "ForemanId",
                table: "Crews");

            migrationBuilder.DropColumn(
                name: "ForemanWorkerId",
                table: "Crews");

            migrationBuilder.AddColumn<string>(
                name: "ForemanName",
                table: "Crews",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Crews",
                type: "TEXT",
                nullable: true);
        }
    }
}
