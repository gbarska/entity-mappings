﻿using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Samurais",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SurName = table.Column<string>(nullable: true),
                    GivenName = table.Column<string>(nullable: true),
                    BetterName_Created = table.Column<DateTime>(nullable: false),
                    BetterName_LastModified = table.Column<DateTime>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samurais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true),
                    SamuraiId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_Samurais_SamuraiId",
                        column: x => x.SamuraiId,
                        principalTable: "Samurais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SamuraiBattle",
                columns: table => new
                {
                    SamuraiId = table.Column<int>(nullable: false),
                    BattleId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamuraiBattle", x => new { x.SamuraiId, x.BattleId });
                    table.ForeignKey(
                        name: "FK_SamuraiBattle_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SamuraiBattle_Samurais_SamuraiId",
                        column: x => x.SamuraiId,
                        principalTable: "Samurais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecretIdentity",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RealName = table.Column<string>(nullable: true),
                    SamuraiId = table.Column<int>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecretIdentity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecretIdentity_Samurais_SamuraiId",
                        column: x => x.SamuraiId,
                        principalTable: "Samurais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_SamuraiId",
                table: "Quotes",
                column: "SamuraiId");

            migrationBuilder.CreateIndex(
                name: "IX_SamuraiBattle_BattleId",
                table: "SamuraiBattle",
                column: "BattleId");

            migrationBuilder.CreateIndex(
                name: "IX_SecretIdentity_SamuraiId",
                table: "SecretIdentity",
                column: "SamuraiId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "SamuraiBattle");

            migrationBuilder.DropTable(
                name: "SecretIdentity");

            migrationBuilder.DropTable(
                name: "Battles");

            migrationBuilder.DropTable(
                name: "Samurais");
        }
    }
}
