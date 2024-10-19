using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeAPI.Migrations;

/// <inheritdoc />
public partial class RemovedTwoColumns : Migration
{
	/// <inheritdoc />
	protected override void Up (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.DropColumn(
			name: "DisplayName",
			table: "Topics");

		_ = migrationBuilder.DropColumn(
			name: "Unit",
			table: "Topics");
	}

	/// <inheritdoc />
	protected override void Down (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.AddColumn<string>(
			name: "DisplayName",
			table: "Topics",
			type: "text",
			nullable: false,
			defaultValue: "");

		_ = migrationBuilder.AddColumn<string>(
			name: "Unit",
			table: "Topics",
			type: "text",
			nullable: false,
			defaultValue: "");
	}
}
