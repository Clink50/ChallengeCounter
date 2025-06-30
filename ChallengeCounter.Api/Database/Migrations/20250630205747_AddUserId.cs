using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChallengeCounter.Api.Database.Migrations;

/// <inheritdoc />
public partial class AddUserId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "user_id",
            table: "workout_log",
            type: "text",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<Guid>(
            name: "user_id",
            table: "workout_log",
            type: "uuid",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "text");
    }
}
