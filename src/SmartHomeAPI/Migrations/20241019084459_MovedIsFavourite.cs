using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHomeAPI.Migrations;

/// <inheritdoc />
public partial class MovedIsFavourite : Migration
{
	/// <inheritdoc />
	protected override void Up (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.DropColumn(
			name: "IsFavourite",
			table: "Measurements");

		_ = migrationBuilder.AddColumn<bool>(
			name: "IsFavourite",
			table: "Topics",
			type: "boolean",
			nullable: false,
			defaultValue: false);
	}

	/// <inheritdoc />
	protected override void Down (MigrationBuilder migrationBuilder)
	{
		_ = migrationBuilder.DropColumn(
			name: "IsFavourite",
			table: "Topics");

		_ = migrationBuilder.AddColumn<bool>(
			name: "IsFavourite",
			table: "Measurements",
			type: "boolean",
			nullable: false,
			defaultValue: false);
	}
}
