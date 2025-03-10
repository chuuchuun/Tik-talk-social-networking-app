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
    public DbSet<Message> Messages { get; set; }
    public DbSet<Chat> Chats{get;set;}
    public DbSet<Post> Posts { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
         builder.Entity<Message>()
        .HasOne(m => m.Chat)
        .WithMany(c => c.messages)
        .HasForeignKey(m => m.chatId)
        .OnDelete(DeleteBehavior.Cascade);
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
