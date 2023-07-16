using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Country> Countries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            string countriesJson = File.ReadAllText("countries.json");
            string personsJson = File.ReadAllText("persons.json");

            List<Country> countries = JsonSerializer.Deserialize<List<Country>>(countriesJson)!;
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson)!;

            foreach(var country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            foreach (var person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }


            //Fluent API

            modelBuilder.Entity<Person>().Property(property => property.Address)
                .HasColumnName("Address")
                .HasColumnType("varchar(80)")
                .HasDefaultValue("Porto");

            // modelBuilder.Entity<Person>().HasIndex(property => property.Address).IsUnique();


            //Table Relations

           /* modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne<Country>(el => el.Country)
                      .WithMany(el => el.Persons)
                      .HasForeignKey(key => key.CountryId);
            });*/
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", person.Id),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@BirthDate", person.BirthDate),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceivedNewsLetter", person.ReceivedNewsLetter)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @Id, @Name, @Email, @BirthDate, " +
                "@Gender, @CountryId, @Address, @ReceivedNewsLetter", parameters);
        }

        public int sp_DeletePerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@Id", person.Id) };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[DeletePerson] @Id", parameters);
        }

        public int sp_UpdatePerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[] 
            {
                new SqlParameter("@Id", person.Id),
                new SqlParameter("@Name", person.Name),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@BirthDate", person.BirthDate),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryId", person.CountryId),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReceivedNewsLetter", person.ReceivedNewsLetter)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[UpdatePerson ]@Id, @Name, @Email, @BirthDate, " +
                "@Gender, @CountryId, @Address, @ReceivedNewsLetter", parameters);
        }
    }
}
