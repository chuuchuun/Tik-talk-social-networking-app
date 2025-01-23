using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace tik_talk.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuth : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
    name: "refreshTokenExpiry",
    table: "AspNetUsers", // Use the actual table name here
    nullable: true,
    oldClrType: typeof(DateTime),
    oldNullable: false);

    }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<DateTime>(
            name: "refreshTokenExpiry",
            table: "AspNetUsers", // The table name
            nullable: false, // Revert it to non-nullable
            oldClrType: typeof(DateTime),
            oldNullable: true); // The previous state was nullable
    }
    }
}
