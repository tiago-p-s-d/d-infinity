using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CampaignId",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CampaignId1",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    campaign_name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_users_CampaignId",
                table: "users",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_users_CampaignId1",
                table: "users",
                column: "CampaignId1");

            migrationBuilder.AddForeignKey(
                name: "FK_users_campaigns_CampaignId",
                table: "users",
                column: "CampaignId",
                principalTable: "campaigns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_campaigns_CampaignId1",
                table: "users",
                column: "CampaignId1",
                principalTable: "campaigns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_campaigns_CampaignId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_campaigns_CampaignId1",
                table: "users");

            migrationBuilder.DropTable(
                name: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_users_CampaignId",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_CampaignId1",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CampaignId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "CampaignId1",
                table: "users");
        }
    }
}
