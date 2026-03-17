using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToGameplayContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "systems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "spells",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "skills",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "races",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "maps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "created_by",
                table: "items",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_systems_created_by",
                table: "systems",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_spells_created_by",
                table: "spells",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_skills_created_by",
                table: "skills",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_races_created_by",
                table: "races",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_maps_created_by",
                table: "maps",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_items_created_by",
                table: "items",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "FK_items_users_created_by",
                table: "items",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_maps_users_created_by",
                table: "maps",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_races_users_created_by",
                table: "races",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_skills_users_created_by",
                table: "skills",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_spells_users_created_by",
                table: "spells",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_systems_users_created_by",
                table: "systems",
                column: "created_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_users_created_by",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_maps_users_created_by",
                table: "maps");

            migrationBuilder.DropForeignKey(
                name: "FK_races_users_created_by",
                table: "races");

            migrationBuilder.DropForeignKey(
                name: "FK_skills_users_created_by",
                table: "skills");

            migrationBuilder.DropForeignKey(
                name: "FK_spells_users_created_by",
                table: "spells");

            migrationBuilder.DropForeignKey(
                name: "FK_systems_users_created_by",
                table: "systems");

            migrationBuilder.DropIndex(
                name: "IX_systems_created_by",
                table: "systems");

            migrationBuilder.DropIndex(
                name: "IX_spells_created_by",
                table: "spells");

            migrationBuilder.DropIndex(
                name: "IX_skills_created_by",
                table: "skills");

            migrationBuilder.DropIndex(
                name: "IX_races_created_by",
                table: "races");

            migrationBuilder.DropIndex(
                name: "IX_maps_created_by",
                table: "maps");

            migrationBuilder.DropIndex(
                name: "IX_items_created_by",
                table: "items");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "systems");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "spells");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "skills");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "races");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "maps");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "items");
        }
    }
}
