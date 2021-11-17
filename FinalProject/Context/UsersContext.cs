using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Context
{
    public class UsersContext : DbContext
    {


    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public const string headRoleName = "Head";
        public const string depHeadRoleName = "DepHead";
        public const string gynecRoleName = "Gynec";
        public const string obstetRoleName = "Obstet";

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            Role head = new Role { Id = 1, Name = headRoleName};
            Role dephead = new Role { Id = 2, Name = depHeadRoleName };
            Role gynec = new Role { Id = 3, Name = gynecRoleName };
            Role obstet = new Role { Id = 4, Name = obstetRoleName };

            User admin = new User { id = 1, PhoneNumber = "992988775715", Password = "123456", RoleId = head.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { head, dephead, gynec, obstet });
            modelBuilder.Entity<User>().HasData(new User[] { admin });
            base.OnModelCreating(modelBuilder);
        }
    }
}

