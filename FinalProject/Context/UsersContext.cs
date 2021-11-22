using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace FinalProject.Context
{
    public class UsersContext : DbContext
    {

        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Doctor> Doctors { get; set; }

        public const string headRoleName = "Head";
        public const string depHeadRoleName = "DepHead";
        public const string gynecRoleName = "Gynec";
        public const string obstetRoleName = "Obstet";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            Role head = new Role { Id = 1, Name = headRoleName, RusName = "Главный врач"};
            Role dephead = new Role { Id = 2, Name = depHeadRoleName, RusName = "Заведующий кафедры" };
            Role gynec = new Role { Id = 3, Name = gynecRoleName, RusName = "Акушер-гинеколог" };
            Role obstet = new Role { Id = 4, Name = obstetRoleName, RusName = "Акушер" };
            User admin = new User { Id = 1, PhoneNumber = "992988775715", HashedPassword = Crypto.HashPassword("123456"), RoleId = head.Id };
            Doctor adminDoc = new Doctor { Id = 1, FirstName = "Назар", LastName = "Абдурахимов", MiddleName = "Рустамович", DateOfBirth = new DateTime(1997, 09, 24), CreatedDate = new DateTime(2021, 11, 19), PassportNumber = "A50724353", Address = "Rudaki 70 apt 80", UserId = admin.Id};
            modelBuilder.Entity<Doctor>().HasData(new Doctor[] { adminDoc });
            modelBuilder.Entity<Role>().HasData(new Role[] { head, dephead, gynec, obstet });
            modelBuilder.Entity<User>().HasData(new User[] { admin });
            base.OnModelCreating(modelBuilder);
        }
    }
}

