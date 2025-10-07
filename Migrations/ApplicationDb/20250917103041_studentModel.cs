using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookMate.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class studentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Last_Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Date_of_Birth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    No_of_Months = table.Column<int>(type: "int", nullable: false),
                    Admission_Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Fees_Amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total_Fees_Amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Student_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact_Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
