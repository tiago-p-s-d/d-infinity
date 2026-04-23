using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterSheetModelSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_sheets_inventories_inventory_id",
                table: "character_sheets");

            migrationBuilder.DropIndex(
                name: "IX_character_sheets_inventory_id",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "backstory",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "characteristics",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "current_hp",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "current_mp",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "inventory_id",
                table: "character_sheets");

            migrationBuilder.DropColumn(
                name: "level",
                table: "character_sheets");

            migrationBuilder.RenameColumn(
                name: "stats",
                table: "character_sheets",
                newName: "values");

            migrationBuilder.RenameColumn(
                name: "max_mp",
                table: "character_sheets",
                newName: "player_id");

            migrationBuilder.RenameColumn(
                name: "max_hp",
                table: "character_sheets",
                newName: "model_id");

            migrationBuilder.CreateTable(
                name: "character_sheet_models",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    definitions = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_character_sheet_models", x => x.id);
                    table.ForeignKey(
                        name: "FK_character_sheet_models_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_character_sheets_model_id",
                table: "character_sheets",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_sheets_player_id",
                table: "character_sheets",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_character_sheet_models_created_by",
                table: "character_sheet_models",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_character_sheets_character_sheet_models_model_id",
                table: "character_sheets",
                column: "model_id",
                principalTable: "character_sheet_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_character_sheets_users_player_id",
                table: "character_sheets",
                column: "player_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_character_sheets_character_sheet_models_model_id",
                table: "character_sheets");

            migrationBuilder.DropForeignKey(
                name: "FK_character_sheets_users_player_id",
                table: "character_sheets");

            migrationBuilder.DropTable(
                name: "character_sheet_models");

            migrationBuilder.DropIndex(
                name: "IX_character_sheets_model_id",
                table: "character_sheets");

            migrationBuilder.DropIndex(
                name: "IX_character_sheets_player_id",
                table: "character_sheets");

            migrationBuilder.RenameColumn(
                name: "values",
                table: "character_sheets",
                newName: "stats");

            migrationBuilder.RenameColumn(
                name: "player_id",
                table: "character_sheets",
                newName: "max_mp");

            migrationBuilder.RenameColumn(
                name: "model_id",
                table: "character_sheets",
                newName: "max_hp");

            migrationBuilder.AddColumn<string>(
                name: "backstory",
                table: "character_sheets",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "characteristics",
                table: "character_sheets",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "current_hp",
                table: "character_sheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "current_mp",
                table: "character_sheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "inventory_id",
                table: "character_sheets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "character_sheets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_character_sheets_inventory_id",
                table: "character_sheets",
                column: "inventory_id");

            migrationBuilder.AddForeignKey(
                name: "FK_character_sheets_inventories_inventory_id",
                table: "character_sheets",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id");
        }
    }
}
