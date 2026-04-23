using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class NewCampaignFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "about",
                table: "campaigns",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "invite_code",
                table: "campaigns",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "system_id",
                table: "campaigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_system_id",
                table: "campaigns",
                column: "system_id");

            migrationBuilder.AddForeignKey(
                name: "FK_campaigns_systems_system_id",
                table: "campaigns",
                column: "system_id",
                principalTable: "systems",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaigns_systems_system_id",
                table: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_campaigns_system_id",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "about",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "invite_code",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "system_id",
                table: "campaigns");

            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "maps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_maps_CampaignId",
                table: "maps",
                column: "CampaignId");

            migrationBuilder.AddForeignKey(
                name: "FK_maps_campaigns_CampaignId",
                table: "maps",
                column: "CampaignId",
                principalTable: "campaigns",
                principalColumn: "id");
        }
    }
}
