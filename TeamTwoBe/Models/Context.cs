using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TeamTwoBe.Models
{
    public class Context : DbContext
    {  
        public DbSet<User> Users { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
    }
}