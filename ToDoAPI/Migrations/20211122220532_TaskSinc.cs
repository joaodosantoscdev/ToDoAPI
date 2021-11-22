 using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoAPI.Migrations
{
    public partial class TaskSinc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "IdTaskApp");

            migrationBuilder.AlterColumn<int>(
                name: "IdTaskApp",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "IdTaskApi",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<bool>(
                name: "Excluded",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "IdTaskApi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IdTaskApi",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Excluded",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "IdTaskApp",
                table: "Tasks",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tasks",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");
        }
    }
}
