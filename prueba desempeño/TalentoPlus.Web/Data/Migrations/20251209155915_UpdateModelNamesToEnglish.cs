using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentoPlus.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelNamesToEnglish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename Empleados table to Employees
            migrationBuilder.RenameTable(
                name: "Empleados",
                newName: "Employees");

            // Rename Departamentos table to Departments
            migrationBuilder.RenameTable(
                name: "Departamentos",
                newName: "Departments");

            // Rename columns in Employees table
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Employees",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Apellido",
                table: "Employees",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Documento",
                table: "Employees",
                newName: "Document");

            migrationBuilder.RenameColumn(
                name: "Cargo",
                table: "Employees",
                newName: "Position");

            migrationBuilder.RenameColumn(
                name: "FechaIngreso",
                table: "Employees",
                newName: "HireDate");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Employees",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "NivelEducativo",
                table: "Employees",
                newName: "EducationLevel");

            migrationBuilder.RenameColumn(
                name: "PerfilProfesional",
                table: "Employees",
                newName: "ProfessionalProfile");

            migrationBuilder.RenameColumn(
                name: "Telefono",
                table: "Employees",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "Direccion",
                table: "Employees",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "DepartamentoId",
                table: "Employees",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Employees",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "Employees",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Employees",
                newName: "UserId");

            // Rename columns in Departments table
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "Departments",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "Departments",
                newName: "Description");

            // Drop and recreate foreign key with new name
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Employees");

            // Rename foreign key index
            migrationBuilder.RenameIndex(
                name: "IX_Empleados_DepartamentoId",
                table: "Employees",
                newName: "IX_Employees_DepartmentId");

            // Recreate foreign key with new name
            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Employees",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Update seed data in Departments
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "IT and Development Department", "Technology" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Personnel Management", "Human Resources" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Commercial and Sales", "Sales" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Accounting and Finance", "Finance" });

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Operations and Logistics", "Operations" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert table names
            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Empleados");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Departamentos");

            // Revert column names in Empleados table
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Empleados",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Empleados",
                newName: "Apellido");

            migrationBuilder.RenameColumn(
                name: "Document",
                table: "Empleados",
                newName: "Documento");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Empleados",
                newName: "Cargo");

            migrationBuilder.RenameColumn(
                name: "HireDate",
                table: "Empleados",
                newName: "FechaIngreso");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Empleados",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "EducationLevel",
                table: "Empleados",
                newName: "NivelEducativo");

            migrationBuilder.RenameColumn(
                name: "ProfessionalProfile",
                table: "Empleados",
                newName: "PerfilProfesional");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Empleados",
                newName: "Telefono");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Empleados",
                newName: "Direccion");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Empleados",
                newName: "DepartamentoId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Empleados",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Empleados",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Empleados",
                newName: "UsuarioId");

            // Revert columns in Departamentos table
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Departamentos",
                newName: "Nombre");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Departamentos",
                newName: "Descripcion");

            // Drop and recreate foreign key with old name
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Departments_DepartmentId",
                table: "Empleados");

            // Revert index names
            migrationBuilder.RenameIndex(
                name: "IX_Employees_DepartmentId",
                table: "Empleados",
                newName: "IX_Empleados_DepartamentoId");

            // Recreate foreign key with old name
            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Departamentos_DepartamentoId",
                table: "Empleados",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Revert seed data
            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Departamento de IT y desarrollo", "Tecnología" });

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Gestión de personal", "Recursos Humanos" });

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Comercial y ventas", "Ventas" });

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Contabilidad y finanzas", "Finanzas" });

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Operaciones y logística", "Operaciones" });
        }
    }
}
