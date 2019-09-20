using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
    public partial class newp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BetterName_Created",
                table: "Samurais");

            migrationBuilder.DropColumn(
                name: "GivenName",
                table: "Samurais");

            migrationBuilder.DropColumn(
                name: "BetterName_LastModified",
                table: "Samurais");

            migrationBuilder.DropColumn(
                name: "SurName",
                table: "Samurais");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BetterName_Created",
                table: "Samurais",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GivenName",
                table: "Samurais",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BetterName_LastModified",
                table: "Samurais",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SurName",
                table: "Samurais",
                nullable: true);
        }
    }
}
