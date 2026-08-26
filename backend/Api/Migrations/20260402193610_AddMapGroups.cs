using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMapGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_maps_campaigns_campaign_id",
                table: "maps");

            migrationBuilder.RenameColumn(
                name: "campaign_id",
                table: "maps",
                newName: "CampaignId");

            migrationBuilder.RenameIndex(
                name: "IX_maps_campaign_id",
                table: "maps",
                newName: "IX_maps_CampaignId");

            migrationBuilder.AlterColumn<int>(
                name: "CampaignId",
                table: "maps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "map_group_id",
                table: "maps",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "map_groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_map_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_map_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_maps_map_group_id",
                table: "maps",
                column: "map_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_map_groups_created_by",
                table: "map_groups",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_maps_campaigns_CampaignId",
                table: "maps",
                column: "CampaignId",
                principalTable: "campaigns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_maps_map_groups_map_group_id",
                table: "maps",
                column: "map_group_id",
                principalTable: "map_groups",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_maps_campaigns_CampaignId",
                table: "maps");

            migrationBuilder.DropForeignKey(
                name: "FK_maps_map_groups_map_group_id",
                table: "maps");

            migrationBuilder.DropTable(
                name: "map_groups");

            migrationBuilder.DropIndex(
                name: "IX_maps_map_group_id",
                table: "maps");

            migrationBuilder.DropColumn(
                name: "map_group_id",
                table: "maps");

            migrationBuilder.RenameColumn(
                name: "CampaignId",
                table: "maps",
                newName: "campaign_id");

            migrationBuilder.RenameIndex(
                name: "IX_maps_CampaignId",
                table: "maps",
                newName: "IX_maps_campaign_id");

            migrationBuilder.AlterColumn<int>(
                name: "campaign_id",
                table: "maps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_maps_campaigns_campaign_id",
                table: "maps",
                column: "campaign_id",
                principalTable: "campaigns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
