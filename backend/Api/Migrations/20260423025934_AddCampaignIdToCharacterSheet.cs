using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignIdToCharacterSheet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "campaign_id",
                table: "character_sheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_character_sheets_campaign_id",
                table: "character_sheets",
                column: "campaign_id");

            migrationBuilder.AddForeignKey(
                name: "FK_character_sheets_campaigns_campaign_id",
                table: "character_sheets",
                column: "campaign_id",
                principalTable: "campaigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_sheets_campaigns_campaign_id",
                table: "character_sheets");

            migrationBuilder.DropIndex(
                name: "IX_character_sheets_campaign_id",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "campaign_id",
                table: "character_sheets");
        }
    }
}
