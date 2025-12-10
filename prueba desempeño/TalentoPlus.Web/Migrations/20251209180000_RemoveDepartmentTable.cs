using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentoPlus.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            // Drop the index on DepartmentId
            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            // Add Department column as varchar to Employees
            migrationBuilder.AddColumn<string>(
                name: "Department_New",
                table: "Employees",
                type: "character varying(100)",
                nullable: true);

            // Migrate data from Departments table to Department column
            migrationBuilder.Sql(
                @"UPDATE ""Employees"" e
                  SET ""Department_New"" = d.""Name""
                  FROM ""Departments"" d
                  WHERE e.""DepartmentId"" = d.""Id"";
                  
                  UPDATE ""Employees"" 
                  SET ""Department_New"" = 'General'
                  WHERE ""Department_New"" IS NULL;");

            // Drop DepartmentId column
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Employees");

            // Rename Department_New to Department
            migrationBuilder.RenameColumn(
                name: "Department_New",
                table: "Employees",
                newName: "Department");

            // Drop Departments table
            migrationBuilder.DropTable(
                name: "Departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate Departments table
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            // Add DepartmentId column back
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "integer",
                nullable: true);

            // Migrate data back from Department string to DepartmentId
            migrationBuilder.Sql(
                @"INSERT INTO ""Departments"" (""Name"", ""CreatedAt"")
                  SELECT DISTINCT ""Department"", NOW()
                  FROM ""Employees""
                  WHERE ""Department"" IS NOT NULL
                  ON CONFLICT DO NOTHING;
                  
                  UPDATE ""Employees"" e
                  SET ""DepartmentId"" = d.""Id""
                  FROM ""Departments"" d
                  WHERE e.""Department"" = d.""Name"";");

            // Drop Department column
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Employees");

            // Recreate foreign key
            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
