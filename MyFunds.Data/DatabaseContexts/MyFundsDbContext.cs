using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyFunds.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFunds.Data.DatabaseContexts
{
    public class MyFundsDbContext : IdentityDbContext<User, UserRole, int>
    {
        public MyFundsDbContext(DbContextOptions<MyFundsDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<FixedAsset> FixedAssets { get; set; }
        public DbSet<FixedAssetArchive> FixedAssetArchives { get; set; }
        public DbSet<MobileAsset> MobileAssets { get; set; }
        public DbSet<MobileAssetArchive> MobileAssetArchives { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Building> Buildings { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Building>()
                .HasOne<Address>(b => b.Address)
                .WithOne(a => a.Building)
                .HasForeignKey<Address>(a => a.BuildingId);

            builder.Entity<FixedAssetArchive>()
                .HasOne<User>(f => f.User)
                .WithMany(u => u.FixedAssetArchives)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FixedAssetArchive>()
                .HasOne<Room>(f => f.Room)
                .WithMany(r => r.FixedAssetArchives)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MobileAssetArchive>()
                .HasOne<User>(m => m.User)
                .WithMany(u => u.MobileAssetArchives)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Room>()
                .Property(r => r.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (RoomType)Enum.Parse(typeof(RoomType), v));

            builder.Entity<FixedAsset>()
                .Property(f => f.Type)
                .HasConversion(
                    v => v.ToString(),
                    v => (FixedAssetType)Enum.Parse(typeof(FixedAssetType), v));
        }
    }
}
