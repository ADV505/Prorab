using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomSiteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_SiteId",
                table: "Rooms",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Sites_SiteId",
                table: "Rooms",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Sites_SiteId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_SiteId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "Rooms");
        }
    }
}
