using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Contracts;
using Wallet.Domain.Entities;

namespace Wallet.Infrastructure.Persitstance
{
    public class ApplicationDbContext : IdentityDbContext<User, ApplicationRole, int>, IApplicationDbContext
    {
        //readonly string _connectionString;
        //readonly IConfiguration _configuration;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();

        public DbSet<Transaction> Transactions => Set<Transaction>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure the User entity
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .HasIndex(x => x.MobileNumber)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(idx => idx.Email)
                .IsUnique(false);

            builder.Entity<User>()
            .Property(u => u.Balance)
            .HasColumnType("decimal(18,2)");

            // Configure the Transaction entity
            builder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany(u => u.SentTransactions)
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.Entity<Transaction>()
                .HasOne(t => t.Recipient)
                .WithMany(u => u.ReceivedTransactions)
                .HasForeignKey(t => t.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
           .Property(u => u.Amount)
           .HasColumnType("decimal(18,2)");


        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
