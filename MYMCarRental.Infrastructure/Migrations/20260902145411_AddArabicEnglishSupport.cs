using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MYMCarRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArabicEnglishSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Cars",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Cars",
                newName: "DescriptionEn");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CarCategories",
                newName: "NameEn");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CarCategories",
                newName: "DescriptionEn");

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "Cars",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "Cars",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePublicId",
                table: "CarCategories",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionAr",
                table: "CarCategories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NameAr",
                table: "CarCategories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DescriptionAr",
                table: "CarCategories");

            migrationBuilder.DropColumn(
                name: "NameAr",
                table: "CarCategories");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "Cars",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "Cars",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "NameEn",
                table: "CarCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DescriptionEn",
                table: "CarCategories",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePublicId",
                table: "CarCategories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
