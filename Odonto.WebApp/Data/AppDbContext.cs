using Microsoft.EntityFrameworkCore;
using Odonto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odonto.WebApp.Data
{
    public class AppDbContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UsePostgreSql(@"host=server;database=PerfectSmile;user id=postgres;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<OrderDetail>().HasKey(p => new { p.OrderID, p.ProductID });
        }

        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<Dentist> Dentists { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientRecord> PatientRecords { get; set; }
        public DbSet<PatientRecordDisease> PatientRecordDiseases { get; set; }
        public DbSet<PatientRecordProcedure> PatientRecordProcedures { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
    }
}
