using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tik_talk.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace tik_talk.Data;

public class ApplicationDBContext : IdentityDbContext<Auth>
{
    public ApplicationDBContext(DbContextOptions dbContextOptions): base(dbContextOptions)
        {
            
        }

    public DbSet<Account> Accounts {get;set;}
    public DbSet<Auth> Auths { get; set; }

    public DbSet<Chat> Chats{get;set;}
    protected override void OnModelCreating(ModelBuilder builder)
    {
         builder.Entity<Chat>()
        .HasOne(c => c.userFirst)
        .WithMany(a => a.chatsAsFirstUser) // Use a distinct navigation property
        .HasForeignKey(c => c.userFirstId)
        .OnDelete(DeleteBehavior.NoAction);

    // Define the relationship for userSecond
    builder.Entity<Chat>()
        .HasOne(c => c.userSecond)
        .WithMany(a => a.chatsAsSecondUser) // Use a distinct navigation property
        .HasForeignKey(c => c.userSecondId)
        .OnDelete(DeleteBehavior.NoAction);

        
        base.OnModelCreating(builder);
        List<IdentityRole> roles = new List<IdentityRole>{
            new IdentityRole{
                Name = "Admin",
                NormalizedName = "ADMIN"
            },
            new IdentityRole{
                Name = "User",
                NormalizedName = "USER"
            },
        };
        builder.Entity<IdentityRole>().HasData(roles);

    }
}
