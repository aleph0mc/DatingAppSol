using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    // Inheritance from
    // IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    // required because the type for Id that we are using is int and not string, otherwise just inheritance from
    // IdentityDbContext
    // would have been enough
    // therefore for Id's better to always use string
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> Options) : base(Options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // required for IdentityServer4

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(r => r.Role)
                    .WithMany(ur => ur.UserRoles)
                    .HasForeignKey(r => r.RoleId)
                    .IsRequired();

                userRole.HasOne(r => r.User)
                    .WithMany(ur => ur.UserRoles)
                    .HasForeignKey(u => u.UserId)
                    .IsRequired();
            });

            builder.Entity<Like>()
                .HasKey(k => new { k.LikerId, k.LikeeId });

            builder.Entity<Like>()
                .HasOne(like => like.Likee)
                .WithMany(user => user.Likers)
                .HasForeignKey(like => like.LikerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                .HasOne(like => like.Liker)
                .WithMany(user => user.Likees)
                .HasForeignKey(like => like.LikerId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            // Global query filter for the photos that are not yet approved.
            // This filter can be ignored in the Linq query adding a .IgnoreQueryFilters() property
            // (check DatingRepository.cs)
            builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);
        }
    }
}
