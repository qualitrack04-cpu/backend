using System;
using Microsoft.EntityFrameworkCore.Migrations;
using QualiTrack.Data;

#nullable disable

namespace QualiTrack.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Standard = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checklists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Standard = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    ClauseRef = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistItems_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClauseRef = table.Column<string>(type: "text", nullable: false),
                    AuditorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditSchedules_AuditPlans_AuditPlanId",
                        column: x => x.AuditPlanId,
                        principalTable: "AuditPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditSchedules_User_AuditorId",
                        column: x => x.AuditorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditSessions_AuditSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "AuditSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditSessions_Checklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "Checklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChecklistItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditResponses_AuditSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "AuditSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditResponses_ChecklistItems_ChecklistItemId",
                        column: x => x.ChecklistItemId,
                        principalTable: "ChecklistItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Findings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ClauseRef = table.Column<string>(type: "text", nullable: false),
                    FoundAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Findings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Findings_AuditSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "AuditSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAPAs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FindingId = table.Column<Guid>(type: "uuid", nullable: false),
                    RootCause = table.Column<string>(type: "text", nullable: false),
                    CorrectiveAction = table.Column<string>(type: "text", nullable: false),
                    PreventiveAction = table.Column<string>(type: "text", nullable: true),
                    PicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPAs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAPAs_Findings_FindingId",
                        column: x => x.FindingId,
                        principalTable: "Findings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CAPAs_User_PicId",
                        column: x => x.PicId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CAPAActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CapaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DoneById = table.Column<Guid>(type: "uuid", nullable: false),
                    DoneAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CAPAActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CAPAActions_CAPAs_CapaId",
                        column: x => x.CapaId,
                        principalTable: "CAPAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CloseOutVerifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CapaId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEffective = table.Column<bool>(type: "boolean", nullable: false),
                    VerificationNotes = table.Column<string>(type: "text", nullable: false),
                    VerifiedById = table.Column<Guid>(type: "uuid", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloseOutVerifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloseOutVerifications_CAPAs_CapaId",
                        column: x => x.CapaId,
                        principalTable: "CAPAs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvidenceFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    StoragePath = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuditResponseId = table.Column<Guid>(type: "uuid", nullable: true),
                    CapaActionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenceFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenceFiles_AuditResponses_AuditResponseId",
                        column: x => x.AuditResponseId,
                        principalTable: "AuditResponses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EvidenceFiles_CAPAActions_CapaActionId",
                        column: x => x.CapaActionId,
                        principalTable: "CAPAActions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditResponses_ChecklistItemId",
                table: "AuditResponses",
                column: "ChecklistItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditResponses_SessionId",
                table: "AuditResponses",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSchedules_AuditorId",
                table: "AuditSchedules",
                column: "AuditorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSchedules_AuditPlanId",
                table: "AuditSchedules",
                column: "AuditPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSessions_ChecklistId",
                table: "AuditSessions",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditSessions_ScheduleId",
                table: "AuditSessions",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPAActions_CapaId",
                table: "CAPAActions",
                column: "CapaId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPAs_FindingId",
                table: "CAPAs",
                column: "FindingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CAPAs_PicId",
                table: "CAPAs",
                column: "PicId");

            migrationBuilder.CreateIndex(
                name: "IX_CAPAs_Status_Deadline",
                table: "CAPAs",
                columns: new[] { "Status", "Deadline" });

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistItems_ChecklistId",
                table: "ChecklistItems",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_CloseOutVerifications_CapaId",
                table: "CloseOutVerifications",
                column: "CapaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceFiles_AuditResponseId",
                table: "EvidenceFiles",
                column: "AuditResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenceFiles_CapaActionId",
                table: "EvidenceFiles",
                column: "CapaActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Findings_SessionId",
                table: "Findings",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Findings_Status",
                table: "Findings",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloseOutVerifications");

            migrationBuilder.DropTable(
                name: "EvidenceFiles");

            migrationBuilder.DropTable(
                name: "AuditResponses");

            migrationBuilder.DropTable(
                name: "CAPAActions");

            migrationBuilder.DropTable(
                name: "ChecklistItems");

            migrationBuilder.DropTable(
                name: "CAPAs");

            migrationBuilder.DropTable(
                name: "Findings");

            migrationBuilder.DropTable(
                name: "AuditSessions");

            migrationBuilder.DropTable(
                name: "AuditSchedules");

            migrationBuilder.DropTable(
                name: "Checklists");

            migrationBuilder.DropTable(
                name: "AuditPlans");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
