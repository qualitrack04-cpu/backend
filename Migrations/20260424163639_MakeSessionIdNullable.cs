using System;
using Microsoft.EntityFrameworkCore.Migrations;
using QualiTrack.Data;

#nullable disable

namespace QualiTrack.Migrations
{
    /// <inheritdoc />
    public partial class MakeSessionIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Findings_AuditSessions_SessionId",
                table: "Findings");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "Findings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_AuditSessions_SessionId",
                table: "Findings",
                column: "SessionId",
                principalTable: "AuditSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Findings_AuditSessions_SessionId",
                table: "Findings");

            migrationBuilder.AlterColumn<Guid>(
                name: "SessionId",
                table: "Findings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Findings_AuditSessions_SessionId",
                table: "Findings",
                column: "SessionId",
                principalTable: "AuditSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
