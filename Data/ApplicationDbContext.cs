using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProsjektoppgaveITE1811Gruppe7.Models.Entities;
using ProsjektoppgaveITE1811Gruppe7.Models.ViewModels;

namespace ProsjektoppgaveITE1811Gruppe7.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {  }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UsersWalletsModel>().HasData(
            new {Id = 1,  Address = "2N4waHy8etWN3i3nHsVaEAQeYHxFPaEpL1Y"  },
            new {Id = 2,  Address = "2NAwud7YExRq6YUxS2DdUwqZ8enSnw5XgKz" },
            new {Id = 3, Address = "2N6wXMPkSrcVq8gvR1fCUVVvhYSUrys2WGr" }
            );;
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<FileModel> Files { get; set; }

        public DbSet<InvoiceModel> Invoice { get; set; }

        public DbSet<ChatMessageModel> Messages { get; set; }

        public DbSet<UsersWalletsModel> UsersWallets { get; set; }

        public DbSet<PaymentsToFrilanserModel> PaymentsToFrilansers { get; set; }
                     
       
    }
}