using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TalentoPlus.Web.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDepartmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Departments table
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            // Add DepartmentId column back to Employees
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Employees",
                type: "integer",
                nullable: true);

            // Insert default department
            migrationBuilder.Sql(
                @"INSERT INTO ""Departments"" (""Name"", ""Description"") 
                  VALUES ('General', 'Default Department') 
                  ON CONFLICT DO NOTHING;");

            // Migrate existing department data from Department column to DepartmentId
            migrationBuilder.Sql(
                @"UPDATE ""Employees"" e
                  SET ""DepartmentId"" = d.""Id""
                  FROM ""Departments"" d
                  WHERE e.""Department"" = d.""Name"" OR (e.""Department"" IS NULL AND d.""Name"" = 'General');");

            // Drop the Department column
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Employees");

            // Create foreign key relationship
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees");

            // Drop the index
            migrationBuilder.DropIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees");

            // Add Department column back
            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Employees",
                type: "character varying(100)",
                nullable: true);

            // Migrate data back
            migrationBuilder.Sql(
                @"UPDATE ""Employees"" e
                  SET ""Department"" = d.""Name""
                  FROM ""Departments"" d
                  WHERE e.""DepartmentId"" = d.""Id"";
                  
                  UPDATE ""Employees"" 
                  SET ""Department"" = 'General'
                  WHERE ""Department"" IS NULL;");

            // Drop DepartmentId column
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Employees");

            // Drop Departments table
            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
