using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ARSAN_Web.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdClusterAMulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Multa_Residencia_id_residencia",
                table: "Multa");

            migrationBuilder.AddColumn<int>(
                name: "id_cluster",
                table: "Multa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Multa_id_cluster_id_residencia",
                table: "Multa",
                columns: new[] { "id_cluster", "id_residencia" });

            migrationBuilder.AddForeignKey(
                name: "FK_Multa_Residencia_id_residencia",
                table: "Multa",
                column: "id_residencia",
                principalTable: "Residencia",
                principalColumn: "id_residencia",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Multa_Residencia_id_residencia",
                table: "Multa");

            migrationBuilder.DropIndex(
                name: "IX_Multa_id_cluster_id_residencia",
                table: "Multa");

            migrationBuilder.DropColumn(
                name: "id_cluster",
                table: "Multa");

            migrationBuilder.AddForeignKey(
                name: "FK_Multa_Residencia_id_residencia",
                table: "Multa",
                column: "id_residencia",
                principalTable: "Residencia",
                principalColumn: "id_residencia",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
