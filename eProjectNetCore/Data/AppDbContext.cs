using eProjectNetCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eProjectNetCore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Account> Account { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<Class> Class { get; set; }

        public DbSet<UserGroup> UserGroup { get; set; }

        public DbSet<eProjectNetCore.Models.Menu> Menu { get; set; }

        public DbSet<eProjectNetCore.Models.Competition> Competition { get; set; }

        public DbSet<eProjectNetCore.Models.User> User { get; set; }
    }
}
