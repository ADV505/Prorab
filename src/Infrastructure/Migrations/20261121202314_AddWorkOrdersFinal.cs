using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkOrdersFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTypes_WorkSections_WorkSectionId",
                table: "WorkTypes");

            migrationBuilder.DropTable(
                name: "WorkSections");

            migrationBuilder.DropIndex(
                name: "IX_WorkTypes_WorkSectionId",
                table: "WorkTypes");

            migrationBuilder.DropColumn(
                name: "WorkSectionId",
                table: "WorkTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkSectionId",
                table: "WorkTypes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkTypes_WorkSectionId",
                table: "WorkTypes",
                column: "WorkSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSections_Name",
                table: "WorkSections",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSections_SortOrder",
                table: "WorkSections",
                column: "SortOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTypes_WorkSections_WorkSectionId",
                table: "WorkTypes",
                column: "WorkSectionId",
                principalTable: "WorkSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
