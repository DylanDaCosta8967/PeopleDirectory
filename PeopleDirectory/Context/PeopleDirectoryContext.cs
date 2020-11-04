using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using PeopleDirectory.Models;

namespace PeopleDirectory.Context
{
    public class PeopleDirectoryContext : DbContext
    {
        public DbSet<PersonDirectoryModel> PersonDirectoryModel { get; set; } 
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries{ get; set; }
        public DbSet<Users> Users { get; set; }
    }
}