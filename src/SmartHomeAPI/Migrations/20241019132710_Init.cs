using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable IDE0053
#pragma warning disable IDE0058
#nullable disable

namespace SmartHomeAPI.Migrations;

[CompilerGenerated]
/// <inheritdoc />
public partial class Init : Migration
{
	/// <inheritdoc />
	protected override void Up (MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "Topics",
			columns: table => new
			{
				Id = table.Column<int>(type: "integer", nullable: false)
					.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
				Name = table.Column<string>(type: "text", nullable: false),
				IsFavourite = table.Column<bool>(type: "boolean", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Topics", x => x.Id);
			});

		migrationBuilder.CreateTable(
			name: "Measurements",
			columns: table => new
			{
				Id = table.Column<int>(type: "integer", nullable: false)
					.Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
				TopicId = table.Column<int>(type: "integer", nullable: false),
				Value = table.Column<string>(type: "text", nullable: false),
				Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Measurements", x => x.Id);
				table.ForeignKey(
					name: "FK_Measurements_Topics_TopicId",
					column: x => x.TopicId,
					principalTable: "Topics",
					principalColumn: "Id",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_Measurements_TopicId",
			table: "Measurements",
			column: "TopicId");
	}

	/// <inheritdoc />
	protected override void Down (MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "Measurements");

		migrationBuilder.DropTable(
			name: "Topics");
	}
}
