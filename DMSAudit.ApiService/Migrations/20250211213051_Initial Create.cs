using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMSAudit.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Criterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    Group = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criterias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CriteriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<short>(type: "INTEGER", nullable: false),
                    Month = table.Column<byte>(type: "INTEGER", nullable: false),
                    CurrentLvl = table.Column<short>(type: "INTEGER", nullable: false),
                    Progress = table.Column<float>(type: "REAL", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Closed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedBy = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    ClosingComment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaStates_Criterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CriteriaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Level_ = table.Column<short>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Levels_Criterias_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criterias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LevelStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Year = table.Column<short>(type: "INTEGER", nullable: false),
                    Month = table.Column<byte>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LevelStates_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmAzon = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AddedBy = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    State = table.Column<short>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedBy = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Criterias_Name",
                table: "Criterias",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaStates_CriteriaId",
                table: "CriteriaStates",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_CriteriaId",
                table: "Levels",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_LevelStates_LevelId",
                table: "LevelStates",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LevelId",
                table: "Projects",
                column: "LevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriteriaStates");

            migrationBuilder.DropTable(
                name: "LevelStates");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Criterias");
        }
    }
}
