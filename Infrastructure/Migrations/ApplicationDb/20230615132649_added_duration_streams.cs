using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.ApplicationDb
{
    public partial class added_duration_streams : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "start",
                table: "Streams",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "end",
                table: "Streams",
                newName: "End");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Streams",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Streams");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Streams",
                newName: "start");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Streams",
                newName: "end");
        }
    }
}
