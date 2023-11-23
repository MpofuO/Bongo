using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bongo.Migrations
{
    /// <inheritdoc />
    public partial class AddedMergeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MergeKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MergeKey",
                table: "AspNetUsers");
        }
    }
}
