using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EF_core_task_4.Migrations
{
    /// <inheritdoc />
    public partial class Seeddatatomanagerandbranche : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "Address", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "وسط البلد، القاهرة", "NBG-CAI-01", "فرع القاهرة الرئيسي" },
                    { 2, "Sporting, Alexandria", "NBG-ALX-02", "فرع الإسكندرية سبورتنج" }
                });

            migrationBuilder.InsertData(
                table: "Managers",
                columns: new[] { "Id", "BranchId", "FirstName", "LastName", "NationalId", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, 1, "أحمد", "علي", "29001011234567", "01001234567" },
                    { 2, 2, "شريف", "نصر", "28503151234567", "01229876543" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Managers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
