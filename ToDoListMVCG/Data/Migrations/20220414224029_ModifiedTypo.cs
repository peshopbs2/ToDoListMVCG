using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoListMVCG.Data.Migrations
{
    public partial class ModifiedTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedtAt",
                table: "ToDoList",
                newName: "ModifiedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "ToDoList",
                newName: "ModifiedtAt");
        }
    }
}
