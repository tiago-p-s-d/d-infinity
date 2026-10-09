using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMapTokensBlobAndCampaignActiveMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "token_image",
                table: "map_tokens",
                type: "longblob",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "current_map_id",
                table: "campaigns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_current_map_id",
                table: "campaigns",
                column: "current_map_id");

            migrationBuilder.AddForeignKey(
                name: "FK_campaigns_maps_current_map_id",
                table: "campaigns",
                column: "current_map_id",
                principalTable: "maps",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_campaigns_maps_current_map_id",
                table: "campaigns");

            migrationBuilder.DropIndex(
                name: "IX_campaigns_current_map_id",
                table: "campaigns");

            migrationBuilder.DropColumn(
                name: "current_map_id",
                table: "campaigns");

            migrationBuilder.AlterColumn<string>(
                name: "token_image",
                table: "map_tokens",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "longblob")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
