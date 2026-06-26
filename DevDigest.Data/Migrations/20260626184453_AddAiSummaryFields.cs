using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevDigest.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAiSummaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AiSummary",
                table: "Articles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAiProcessed",
                table: "Articles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KeyTakeaways",
                table: "Articles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AiSummary",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "IsAiProcessed",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "KeyTakeaways",
                table: "Articles");
        }
    }
}
