using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using LibaryApplication2.Models;

namespace LibaryApplication2.Context
{
    public class LibraryDb : DbContext
    {
        public LibraryDb() : base("name=DBCS") { }
        public DbSet<Book> Books { get; set; }
    }
}