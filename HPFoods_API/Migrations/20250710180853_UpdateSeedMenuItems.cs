using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPFoods_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Image",
                value: "images/spring_roll.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Image",
                value: "images/samosa.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Image",
                value: "images/soup.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Image",
                value: "images/noodles.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Image",
                value: "images/pav_bhaji.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Image",
                value: "images/pizza.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Image",
                value: "images/mango.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Image",
                value: "images/carrot.jpg");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9L,
                column: "Image",
                value: "images/sweet_rolls.jpg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Image",
                value: "");

            migrationBuilder.UpdateData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: 9L,
                column: "Image",
                value: "");
        }
    }
}
