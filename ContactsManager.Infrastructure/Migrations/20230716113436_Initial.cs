using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true, defaultValue: "Porto"),
                    ReceivedNewsLetter = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("158f59f7-5aad-4c8e-8bd0-ba95f4033887"), "Italy" },
                    { new Guid("87c7d73a-8383-4b22-b728-0c0f1bb6eac0"), "Argentina" },
                    { new Guid("8a05aaee-3985-47e2-8034-f05fa0e8d2ea"), "Australia" },
                    { new Guid("a4e032a1-9fc6-4f22-be31-73daf564ad41"), "USA" },
                    { new Guid("ea75f617-0e2e-41d8-aa52-0caac5dd9072"), "Portugal" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Address", "BirthDate", "CountryId", "Email", "Gender", "Name", "ReceivedNewsLetter" },
                values: new object[,]
                {
                    { new Guid("0d01ee24-e384-42d5-b4ee-a7b7a0d4aa45"), "Águas Santas", new DateTime(1992, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("ea75f617-0e2e-41d8-aa52-0caac5dd9072"), "jorge.basto@hotmail.com", "Male", "Jorge Basto", true },
                    { new Guid("5f1e1be5-26ea-4bdc-88fb-78102955e0af"), "Maia", new DateTime(1964, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("158f59f7-5aad-4c8e-8bd0-ba95f4033887"), "j.santos_1964@hotmail.com", "Male", "Joaquim Santos", true },
                    { new Guid("71cb2bb8-dd80-4d7c-9dba-422489b8fa7e"), "Maia", new DateTime(1991, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("8a05aaee-3985-47e2-8034-f05fa0e8d2ea"), "adelia.santos@hotmail.com", "Female", "Adélia Santos", false },
                    { new Guid("d7271c89-a711-4ce5-b8b1-da7288010cf6"), "Maia", new DateTime(1991, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("ea75f617-0e2e-41d8-aa52-0caac5dd9072"), "j.p.santos21@hotmail.com", "Male", "João Santos", true },
                    { new Guid("fb7a7144-68a8-4b4e-8e7f-6ea77610a48c"), "Castêlo da Maia", new DateTime(1991, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("a4e032a1-9fc6-4f22-be31-73daf564ad41"), "claudia.silva@hotmail.com", "Female", "Cláudia Silva", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_CountryId",
                table: "Persons",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
