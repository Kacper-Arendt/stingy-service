using Auth.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Database;

    public class UsersEfContext : IdentityDbContext<User>
    {
        public UsersEfContext(DbContextOptions<UsersEfContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("auth");
        }
    }
