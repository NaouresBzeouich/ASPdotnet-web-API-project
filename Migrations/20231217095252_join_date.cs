using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_back_end.Migrations
{
    public partial class join_date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "JoinDate",
                table: "AspNetUsers",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinDate",
                table: "AspNetUsers");
        }
    }
}
