using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListAPI.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToTodoItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old single-column index (will be replaced by composite index)
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_ListId",
                table: "TodoItems");

            // Add Order column with default value of 0
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "TodoItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // Initialize existing items with order based on CreatedDate within each list
            // This SQL assigns sequential order (0, 1, 2, ...) to items within each list,
            // ordered by CreatedDate (oldest first)
            // Note: "Order" is a reserved keyword in SQLite, so it must be escaped with double quotes
            migrationBuilder.Sql(@"
                UPDATE TodoItems
                SET ""Order"" = (
                    SELECT COUNT(*)
                    FROM TodoItems AS t2
                    WHERE t2.ListId = TodoItems.ListId
                      AND (t2.CreatedDate < TodoItems.CreatedDate
                           OR (t2.CreatedDate = TodoItems.CreatedDate AND t2.Id < TodoItems.Id))
                );
            ");

            // Create composite index on (ListId, Order) for efficient ordering queries
            // This index also serves queries filtering by ListId alone (leftmost prefix)
            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ListId_Order",
                table: "TodoItems",
                columns: new[] { "ListId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoItems_ListId_Order",
                table: "TodoItems");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "TodoItems");

            migrationBuilder.CreateIndex(
                name: "IX_TodoItems_ListId",
                table: "TodoItems",
                column: "ListId");
        }
    }
}
