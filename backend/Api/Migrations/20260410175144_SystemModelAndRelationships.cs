using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class SystemModelAndRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "items_id",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "maps_id",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "races_id",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "skills_id",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "spells_id",
                table: "systems");

            migrationBuilder.CreateTable(
                name: "SystemItemGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    ItemGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemItemGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemItemGroups_item_groups_ItemGroupId",
                        column: x => x.ItemGroupId,
                        principalTable: "item_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemItemGroups_systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemMapGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    MapGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMapGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemMapGroups_map_groups_MapGroupId",
                        column: x => x.MapGroupId,
                        principalTable: "map_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemMapGroups_systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemRaceGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    RaceGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRaceGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemRaceGroups_race_groups_RaceGroupId",
                        column: x => x.RaceGroupId,
                        principalTable: "race_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemRaceGroups_systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemSkillGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    SkillGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSkillGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSkillGroups_skill_groups_SkillGroupId",
                        column: x => x.SkillGroupId,
                        principalTable: "skill_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemSkillGroups_systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemSpellGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    SpellGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSpellGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSpellGroups_spell_groups_SpellGroupId",
                        column: x => x.SpellGroupId,
                        principalTable: "spell_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemSpellGroups_systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "systems",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SystemItemGroups_ItemGroupId",
                table: "SystemItemGroups",
                column: "ItemGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemItemGroups_SystemId_ItemGroupId",
                table: "SystemItemGroups",
                columns: new[] { "SystemId", "ItemGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemMapGroups_MapGroupId",
                table: "SystemMapGroups",
                column: "MapGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemMapGroups_SystemId_MapGroupId",
                table: "SystemMapGroups",
                columns: new[] { "SystemId", "MapGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemRaceGroups_RaceGroupId",
                table: "SystemRaceGroups",
                column: "RaceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemRaceGroups_SystemId_RaceGroupId",
                table: "SystemRaceGroups",
                columns: new[] { "SystemId", "RaceGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSkillGroups_SkillGroupId",
                table: "SystemSkillGroups",
                column: "SkillGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSkillGroups_SystemId_SkillGroupId",
                table: "SystemSkillGroups",
                columns: new[] { "SystemId", "SkillGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSpellGroups_SpellGroupId",
                table: "SystemSpellGroups",
                column: "SpellGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSpellGroups_SystemId_SpellGroupId",
                table: "SystemSpellGroups",
                columns: new[] { "SystemId", "SpellGroupId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemItemGroups");

            migrationBuilder.DropTable(
                name: "SystemMapGroups");

            migrationBuilder.DropTable(
                name: "SystemRaceGroups");

            migrationBuilder.DropTable(
                name: "SystemSkillGroups");

            migrationBuilder.DropTable(
                name: "SystemSpellGroups");

            migrationBuilder.AddColumn<int>(
                name: "items_id",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "maps_id",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "races_id",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skills_id",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "spells_id",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
