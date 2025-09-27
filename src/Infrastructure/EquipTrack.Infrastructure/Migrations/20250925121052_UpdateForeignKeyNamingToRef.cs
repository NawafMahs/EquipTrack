using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EquipTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyNamingToRef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreventiveMaintenances_Assets_AssetId",
                table: "PreventiveMaintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_PreventiveMaintenances_Users_AssignedToUserId",
                table: "PreventiveMaintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Assets_AssetId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_AssignedToUserId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_CreatedByUserId",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_Assets_AssetId",
                table: "WorkOrderSpareParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_SpareParts_SparePartId",
                table: "WorkOrderSpareParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_WorkOrders_WorkOrderId",
                table: "WorkOrderSpareParts");

            migrationBuilder.RenameColumn(
                name: "WorkOrderId",
                table: "WorkOrderSpareParts",
                newName: "WorkOrderRef");

            migrationBuilder.RenameColumn(
                name: "SparePartId",
                table: "WorkOrderSpareParts",
                newName: "SparePartRef");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "WorkOrderSpareParts",
                newName: "AssetRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_WorkOrderId_SparePartId",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_WorkOrderRef_SparePartRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_SparePartId",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_SparePartRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_AssetId",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_AssetRef");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "WorkOrders",
                newName: "CreatedByUserRef");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "WorkOrders",
                newName: "AssignedToUserRef");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "WorkOrders",
                newName: "AssetRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_CreatedByUserId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_CreatedByUserRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssignedToUserId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssignedToUserRef");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssetId",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssetRef");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserId",
                table: "PreventiveMaintenances",
                newName: "AssignedToUserRef");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "PreventiveMaintenances",
                newName: "AssetRef");

            migrationBuilder.RenameIndex(
                name: "IX_PreventiveMaintenances_AssignedToUserId",
                table: "PreventiveMaintenances",
                newName: "IX_PreventiveMaintenances_AssignedToUserRef");

            migrationBuilder.RenameIndex(
                name: "IX_PreventiveMaintenances_AssetId_IsActive",
                table: "PreventiveMaintenances",
                newName: "IX_PreventiveMaintenances_AssetRef_IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_PreventiveMaintenances_Assets_AssetRef",
                table: "PreventiveMaintenances",
                column: "AssetRef",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreventiveMaintenances_Users_AssignedToUserRef",
                table: "PreventiveMaintenances",
                column: "AssignedToUserRef",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Assets_AssetRef",
                table: "WorkOrders",
                column: "AssetRef",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_AssignedToUserRef",
                table: "WorkOrders",
                column: "AssignedToUserRef",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_CreatedByUserRef",
                table: "WorkOrders",
                column: "CreatedByUserRef",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_Assets_AssetRef",
                table: "WorkOrderSpareParts",
                column: "AssetRef",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_SpareParts_SparePartRef",
                table: "WorkOrderSpareParts",
                column: "SparePartRef",
                principalTable: "SpareParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_WorkOrders_WorkOrderRef",
                table: "WorkOrderSpareParts",
                column: "WorkOrderRef",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreventiveMaintenances_Assets_AssetRef",
                table: "PreventiveMaintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_PreventiveMaintenances_Users_AssignedToUserRef",
                table: "PreventiveMaintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Assets_AssetRef",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_AssignedToUserRef",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Users_CreatedByUserRef",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_Assets_AssetRef",
                table: "WorkOrderSpareParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_SpareParts_SparePartRef",
                table: "WorkOrderSpareParts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderSpareParts_WorkOrders_WorkOrderRef",
                table: "WorkOrderSpareParts");

            migrationBuilder.RenameColumn(
                name: "WorkOrderRef",
                table: "WorkOrderSpareParts",
                newName: "WorkOrderId");

            migrationBuilder.RenameColumn(
                name: "SparePartRef",
                table: "WorkOrderSpareParts",
                newName: "SparePartId");

            migrationBuilder.RenameColumn(
                name: "AssetRef",
                table: "WorkOrderSpareParts",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_WorkOrderRef_SparePartRef",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_WorkOrderId_SparePartId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_SparePartRef",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_SparePartId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrderSpareParts_AssetRef",
                table: "WorkOrderSpareParts",
                newName: "IX_WorkOrderSpareParts_AssetId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserRef",
                table: "WorkOrders",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserRef",
                table: "WorkOrders",
                newName: "AssignedToUserId");

            migrationBuilder.RenameColumn(
                name: "AssetRef",
                table: "WorkOrders",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_CreatedByUserRef",
                table: "WorkOrders",
                newName: "IX_WorkOrders_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssignedToUserRef",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkOrders_AssetRef",
                table: "WorkOrders",
                newName: "IX_WorkOrders_AssetId");

            migrationBuilder.RenameColumn(
                name: "AssignedToUserRef",
                table: "PreventiveMaintenances",
                newName: "AssignedToUserId");

            migrationBuilder.RenameColumn(
                name: "AssetRef",
                table: "PreventiveMaintenances",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_PreventiveMaintenances_AssignedToUserRef",
                table: "PreventiveMaintenances",
                newName: "IX_PreventiveMaintenances_AssignedToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_PreventiveMaintenances_AssetRef_IsActive",
                table: "PreventiveMaintenances",
                newName: "IX_PreventiveMaintenances_AssetId_IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_PreventiveMaintenances_Assets_AssetId",
                table: "PreventiveMaintenances",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PreventiveMaintenances_Users_AssignedToUserId",
                table: "PreventiveMaintenances",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Assets_AssetId",
                table: "WorkOrders",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_AssignedToUserId",
                table: "WorkOrders",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Users_CreatedByUserId",
                table: "WorkOrders",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_Assets_AssetId",
                table: "WorkOrderSpareParts",
                column: "AssetId",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_SpareParts_SparePartId",
                table: "WorkOrderSpareParts",
                column: "SparePartId",
                principalTable: "SpareParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderSpareParts_WorkOrders_WorkOrderId",
                table: "WorkOrderSpareParts",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
