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
        public DbSet<Contact> Contact { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<UserReview> UserReviews { get; set; }
        public DbSet<PremiumBilling> PremiumBilling { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(x => x.Collection).WithMany().Map(x =>
            {
                x.ToTable("Collection");
            });
            modelBuilder.Entity<User>().HasMany(x => x.Wishlist).WithMany().Map(x =>
            {
                x.ToTable("Wishlist");
            });
            modelBuilder.Entity<User>().HasMany(x => x.ShoppingCart).WithMany().Map(x =>
            {
                x.ToTable("ShoppingCart");
            });
            modelBuilder.Entity<User>().HasMany(x => x.Watchlist).WithMany().Map(x =>
            {
                x.ToTable("Watchlist");
            });
            modelBuilder.Entity<User>().HasMany(x => x.Follower).WithMany().Map(x =>
            {
                x.ToTable("Followers");
                x.MapLeftKey("Follower");
                x.MapRightKey("Following");
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}