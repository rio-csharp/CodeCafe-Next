using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeCafe.Modules.Platform.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddPersonalWorkspaceFoundation : Migration
{
    private static readonly string[] OwnerUserKindColumns = ["OwnerUserId", "Kind"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Workspaces",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                OwnerUserId = table.Column<Guid>(type: "TEXT", nullable: false),
                Name = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                Kind = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Workspaces", x => x.Id);
                table.ForeignKey(
                    name: "FK_Workspaces_Users_OwnerUserId",
                    column: x => x.OwnerUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Workspaces_OwnerUserId_Kind",
            table: "Workspaces",
            columns: OwnerUserKindColumns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Workspaces");
    }
}
