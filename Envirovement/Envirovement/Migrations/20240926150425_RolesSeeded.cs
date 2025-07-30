using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Envirovement.Migrations
{
    public partial class RolesSeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3dd21ccb-92b7-4105-951e-885659e9431c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c945ba1-cfe7-406a-afaa-ade6ac516087");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a52dd9aa-fb8e-4f36-be60-672e231a3f63");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c090d7ee-7bf0-479e-af73-139691d31588");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2a1034ad-1cde-4df9-8ca7-b9016069cf5f", "2", "User", "User" },
                    { "6c94de71-d401-4fb8-bce5-d34be8fb8773", "3", "Operator", "Operator" },
                    { "a7723275-ba0a-4614-b8a1-f1b0c2ec0bf6", "3", "HR", "HR" },
                    { "fe3bc994-8d30-4a10-a91e-fcdadf6633cc", "1", "Admin", "Admin" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2a1034ad-1cde-4df9-8ca7-b9016069cf5f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c94de71-d401-4fb8-bce5-d34be8fb8773");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7723275-ba0a-4614-b8a1-f1b0c2ec0bf6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe3bc994-8d30-4a10-a91e-fcdadf6633cc");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3dd21ccb-92b7-4105-951e-885659e9431c", "3", "Operator", "Operator" },
                    { "6c945ba1-cfe7-406a-afaa-ade6ac516087", "2", "User", "User" },
                    { "a52dd9aa-fb8e-4f36-be60-672e231a3f63", "3", "HR", "HR" },
                    { "c090d7ee-7bf0-479e-af73-139691d31588", "1", "Admin", "Admin" }
                });
        }
    }
}
