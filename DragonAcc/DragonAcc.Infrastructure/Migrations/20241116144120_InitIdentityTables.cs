using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonAcc.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuctionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuctionId = table.Column<int>(nullable: true),
                    UserID = table.Column<int>(nullable: true),
                    RaisePrice = table.Column<int>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionDetails");
        }

    }
}
