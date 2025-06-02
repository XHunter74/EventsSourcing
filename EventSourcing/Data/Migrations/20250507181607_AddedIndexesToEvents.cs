using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventSourcing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexesToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateId",
                table: "Events",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_AggregateType",
                table: "Events",
                column: "AggregateType");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Created",
                table: "Events",
                column: "Created");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_AggregateId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_AggregateType",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_Created",
                table: "Events");
        }
    }
}
