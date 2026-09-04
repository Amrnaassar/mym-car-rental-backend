using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MYMCarRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyBookingLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Locations_PickupLocationId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_PickupLocationId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PickupLocationId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Bookings",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "PickupLocationId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PickupLocationId",
                table: "Bookings",
                column: "PickupLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Locations_PickupLocationId",
                table: "Bookings",
                column: "PickupLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
