using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class MoneyPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Kontrola i zmiana schematu odbywają się w tej samej transakcji migracji.
            // Blokady utrzymują sprawdzone dane do końca operacji.
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [Products] WITH (TABLOCKX, HOLDLOCK) WHERE [Price] <> ROUND([Price], 2, 1))
                    OR EXISTS (SELECT 1 FROM [Orders] WITH (TABLOCKX, HOLDLOCK) WHERE [TotalAmount] <> ROUND([TotalAmount], 2, 1))
                    OR EXISTS (SELECT 1 FROM [Invoices] WITH (TABLOCKX, HOLDLOCK) WHERE [Amount] <> ROUND([Amount], 2, 1))
                BEGIN
                    THROW 51001, N'Migracja MoneyPrecision przerwana: istnieją ceny lub kwoty z więcej niż dwoma miejscami po przecinku. Dane pozostają bez zmian; wymagają jawnego rozstrzygnięcia przed migracją.', 1;
                END;
                """);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(31,2)",
                precision: 31,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)",
                oldPrecision: 28,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(31,2)",
                precision: 31,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)",
                oldPrecision: 28,
                oldScale: 8);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Invoices",
                type: "decimal(31,2)",
                precision: 31,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,8)",
                oldPrecision: 28,
                oldScale: 8);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Products",
                type: "decimal(28,8)",
                precision: 28,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(31,2)",
                oldPrecision: 31,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(28,8)",
                precision: 28,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(31,2)",
                oldPrecision: 31,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Invoices",
                type: "decimal(28,8)",
                precision: 28,
                scale: 8,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(31,2)",
                oldPrecision: 31,
                oldScale: 2);
        }
    }
}
