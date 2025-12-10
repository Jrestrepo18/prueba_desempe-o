using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalentoPlus.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixEmployeeDataMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add temporary columns to hold corrected data
            migrationBuilder.AddColumn<string>(
                name: "FirstName_Temp",
                table: "Employees",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName_Temp",
                table: "Employees",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Document_Temp",
                table: "Employees",
                type: "character varying(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email_Temp",
                table: "Employees",
                type: "character varying(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Temp",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone_Temp",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Position_Temp",
                table: "Employees",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary_Temp",
                table: "Employees",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status_Temp",
                table: "Employees",
                type: "character varying(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel_Temp",
                table: "Employees",
                type: "character varying(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfessionalProfile_Temp",
                table: "Employees",
                type: "text",
                nullable: true);

            // Copy and reorganize data from incorrect columns to temp columns
            migrationBuilder.Sql(@"
                UPDATE ""Employees"" SET
                    ""FirstName_Temp"" = ""LastName"",
                    ""LastName_Temp"" = ""Document"",
                    ""Document_Temp"" = ""FirstName"",
                    ""Email_Temp"" = ""Position"",
                    ""Position_Temp"" = ""Status"",
                    ""Status_Temp"" = ""EducationLevel"",
                    ""EducationLevel_Temp"" = ""ProfessionalProfile"",
                    ""Phone_Temp"" = CAST(""Salary"" AS text),
                    ""Address_Temp"" = ""ProfessionalProfile"",
                    ""ProfessionalProfile_Temp"" = ""Phone"",
                    ""Salary_Temp"" = 0
                WHERE TRUE;
            ");

            // Now copy temp data back to original columns
            migrationBuilder.Sql(@"
                UPDATE ""Employees"" SET
                    ""FirstName"" = ""FirstName_Temp"",
                    ""LastName"" = ""LastName_Temp"",
                    ""Document"" = ""Document_Temp"",
                    ""Email"" = ""Email_Temp"",
                    ""Position"" = ""Position_Temp"",
                    ""Status"" = ""Status_Temp"",
                    ""EducationLevel"" = ""EducationLevel_Temp"",
                    ""Phone"" = ""Phone_Temp"",
                    ""Address"" = ""Address_Temp"",
                    ""ProfessionalProfile"" = ""ProfessionalProfile_Temp"",
                    ""Salary"" = COALESCE(""Salary_Temp"", 0)
                WHERE TRUE;
            ");

            // Drop temporary columns
            migrationBuilder.DropColumn(
                name: "FirstName_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastName_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Document_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Email_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Address_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Phone_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Position_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Salary_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Status_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "EducationLevel_Temp",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ProfessionalProfile_Temp",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // This migration cannot be safely reversed without losing data organization
        }
    }
}
