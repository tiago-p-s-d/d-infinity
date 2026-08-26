using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupsStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ac",
                table: "items");

            migrationBuilder.DropColumn(
                name: "damage",
                table: "items");

            migrationBuilder.RenameColumn(
                name: "modifier",
                table: "items",
                newName: "definitions");

            migrationBuilder.AddColumn<int>(
                name: "spell_group_id",
                table: "spells",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "skill_group_id",
                table: "skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "race_group_id",
                table: "races",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "item_group_id",
                table: "items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "currencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "class_groups",
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
                    table.PrimaryKey("PK_class_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_class_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "item_groups",
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
                    table.PrimaryKey("PK_item_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_item_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "race_groups",
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
                    table.PrimaryKey("PK_race_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_race_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "skill_groups",
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
                    table.PrimaryKey("PK_skill_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_skill_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "spell_groups",
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
                    table.PrimaryKey("PK_spell_groups", x => x.id);
                    table.ForeignKey(
                        name: "FK_spell_groups_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_verifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsUsed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_verifications", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "classes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    about = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    definitions = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    class_group_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_classes", x => x.id);
                    table.ForeignKey(
                        name: "FK_classes_class_groups_class_group_id",
                        column: x => x.class_group_id,
                        principalTable: "class_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_classes_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_spells_spell_group_id",
                table: "spells",
                column: "spell_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_skills_skill_group_id",
                table: "skills",
                column: "skill_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_races_race_group_id",
                table: "races",
                column: "race_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_items_item_group_id",
                table: "items",
                column: "item_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_currencies_created_by",
                table: "currencies",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_class_groups_created_by",
                table: "class_groups",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_classes_class_group_id",
                table: "classes",
                column: "class_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_classes_created_by",
                table: "classes",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_item_groups_created_by",
                table: "item_groups",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_race_groups_created_by",
                table: "race_groups",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_skill_groups_created_by",
                table: "skill_groups",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_spell_groups_created_by",
                table: "spell_groups",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_currencies_users_created_by",
                table: "currencies",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_item_groups_item_group_id",
                table: "items",
                column: "item_group_id",
                principalTable: "item_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_races_race_groups_race_group_id",
                table: "races",
                column: "race_group_id",
                principalTable: "race_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_skill_groups_skill_group_id",
                table: "skills",
                column: "skill_group_id",
                principalTable: "skill_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_spells_spell_groups_spell_group_id",
                table: "spells",
                column: "spell_group_id",
                principalTable: "spell_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_currencies_users_created_by",
                table: "currencies");

            migrationBuilder.DropForeignKey(
                name: "FK_items_item_groups_item_group_id",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_races_race_groups_race_group_id",
                table: "races");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_skill_groups_skill_group_id",
                table: "skills");

            migrationBuilder.DropForeignKey(
                name: "FK_spells_spell_groups_spell_group_id",
                table: "spells");

            migrationBuilder.DropTable(
                name: "classes");

            migrationBuilder.DropTable(
                name: "item_groups");

            migrationBuilder.DropTable(
                name: "race_groups");

            migrationBuilder.DropTable(
                name: "skill_groups");

            migrationBuilder.DropTable(
                name: "spell_groups");

            migrationBuilder.DropTable(
                name: "user_verifications");

            migrationBuilder.DropTable(
                name: "class_groups");

            migrationBuilder.DropIndex(
                name: "IX_spells_spell_group_id",
                table: "spells");

            migrationBuilder.DropIndex(
                name: "IX_skills_skill_group_id",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "IX_races_race_group_id",
                table: "races");

            migrationBuilder.DropIndex(
                name: "IX_items_item_group_id",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_currencies_created_by",
                table: "currencies");

            migrationBuilder.DropColumn(
                name: "spell_group_id",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "skill_group_id",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "race_group_id",
                table: "races");

            migrationBuilder.DropColumn(
                name: "item_group_id",
                table: "items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "currencies");

            migrationBuilder.RenameColumn(
                name: "definitions",
                table: "items",
                newName: "modifier");

            migrationBuilder.AddColumn<int>(
                name: "ac",
                table: "items",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "damage",
                table: "items",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
