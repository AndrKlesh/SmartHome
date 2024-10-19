using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SmartHomeAPI.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
	/// <inheritdoc />
	protected override void Up (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.CreateTable(
			name: "Topics",
			columns: table => new
			{
				Id = table.Column<int>(type: "integer", nullable: false)
					.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
				Name = table.Column<string>(type: "text", nullable: false),
				DisplayName = table.Column<string>(type: "text", nullable: false),
				Unit = table.Column<string>(type: "text", nullable: false)
			},
			constraints: table => table.PrimaryKey("PK_Topics", x => x.Id));

		_ = migrationBuilder.CreateTable(
			name: "Measurements",
			columns: table => new
			{
				Id = table.Column<int>(type: "integer", nullable: false)
					.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
				TopicId = table.Column<int>(type: "integer", nullable: false),
				Value = table.Column<string>(type: "text", nullable: false),
				Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
				IsFavourite = table.Column<bool>(type: "boolean", nullable: false)
			},
			constraints: table =>
			{
				_ = table.PrimaryKey("PK_Measurements", x => x.Id);
				_ = table.ForeignKey(
					name: "FK_Measurements_Topics_TopicId",
					column: x => x.TopicId,
					principalTable: "Topics",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		_ = migrationBuilder.CreateIndex(
			name: "IX_Measurements_TopicId",
			table: "Measurements",
			column: "TopicId");
	}

	/// <inheritdoc />
	protected override void Down (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.DropTable(
			name: "Measurements");

		_ = migrationBuilder.DropTable(
			name: "Topics");
	}
}
