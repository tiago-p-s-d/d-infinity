using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class CreateCampaignMembersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_campaigns_CampaignId",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_campaigns_CampaignId1",
                table: "users");

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

            migrationBuilder.CreateTable(
                name: "campaign_users",
                columns: table => new
                {
                    id_usr_cmpg = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    id_campaign = table.Column<int>(type: "int", nullable: false),
                    is_dm = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_users", x => x.id_usr_cmpg);
                    table.ForeignKey(
                        name: "FK_campaign_users_campaigns_id_campaign",
                        column: x => x.id_campaign,
                        principalTable: "campaigns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_campaign_users_users_id_user",
                        column: x => x.id_user,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_users_id_campaign",
                table: "campaign_users",
                column: "id_campaign");

            migrationBuilder.CreateIndex(
                name: "IX_campaign_users_id_user",
                table: "campaign_users",
                column: "id_user");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_users");

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
    }
}
