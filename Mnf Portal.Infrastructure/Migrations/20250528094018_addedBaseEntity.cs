using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mnf_Portal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "News_Id",
                table: "prtl_News",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "prtl_News",
                newName: "News_Id");
        }
    }
}
