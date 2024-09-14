using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookStoreApp.API.Migrations
{
    /// <inheritdoc />
    public partial class SeededDefaultUserAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "eed7a290-74f9-40d0-b5c7-501c84c0c064", null, "Admin", "ADMIN" },
                    { "f501824d-a20a-4524-ae40-3450fdaa3f2a", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "789b5b6d-e101-4b83-8af0-3e57cc91f373", 0, "c7d1178d-91f3-4276-9927-b59428d63fe4", "admin@bookstore.com", false, "System", "Admin", false, null, "ADMIN@BOOKSTORE.COM", "ADMIN@BOOKSTORE.COM", "AQAAAAIAAYagAAAAELUu+JV8Hq3Ju8bM2ugc4L42A0NvC4wiHWRQhq/lRffX37E2sbZenDaj8IDCV5+o6w==", null, false, "3a74a9c7-1a88-4091-b4f5-684464babf90", false, "admin@bookstore.com" },
                    { "88cb08cd-bdb1-4795-9759-8de1471edee9", 0, "0672ab5f-7faf-4c98-9e87-2782533bddf9", "user@bookstore.com", false, "System", "User", false, null, "USER@BOOKSTORE.COM", "USER@BOOKSTORE.COM", "AQAAAAIAAYagAAAAEHRDeAZ7cc3xWhG5e0UhqXS89bEwTAf2ZlP3DSSfGp2EIbnqoXoOr3a0gUN7OBUjiQ==", null, false, "65857d9c-b520-4821-98a3-1ad8a28b5741", false, "user@bookstore.com" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "eed7a290-74f9-40d0-b5c7-501c84c0c064", "789b5b6d-e101-4b83-8af0-3e57cc91f373" },
                    { "f501824d-a20a-4524-ae40-3450fdaa3f2a", "88cb08cd-bdb1-4795-9759-8de1471edee9" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "eed7a290-74f9-40d0-b5c7-501c84c0c064", "789b5b6d-e101-4b83-8af0-3e57cc91f373" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "f501824d-a20a-4524-ae40-3450fdaa3f2a", "88cb08cd-bdb1-4795-9759-8de1471edee9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eed7a290-74f9-40d0-b5c7-501c84c0c064");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f501824d-a20a-4524-ae40-3450fdaa3f2a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "789b5b6d-e101-4b83-8af0-3e57cc91f373");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "88cb08cd-bdb1-4795-9759-8de1471edee9");
        }
    }
}
