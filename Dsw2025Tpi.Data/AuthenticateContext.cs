using Dsw2025Tpi.Data.Identity;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data;

public class AuthenticateContext : IdentityDbContext<IdentityUserExtension>
{
    public AuthenticateContext(DbContextOptions<AuthenticateContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Ignore<Customer>();
        builder.Ignore<Order>();
        builder.Ignore<OrderItem>();
        builder.Ignore<Product>();
        builder.Ignore<EntityBase>();
        builder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.ToTable("Customers", tb => tb.ExcludeFromMigrations());

        });

        builder.Entity<IdentityUserExtension>(b => { 
            b.ToTable("Usuarios");
            b.HasOne<Customer>()
                .WithOne()
                .HasForeignKey<IdentityUserExtension>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

        });
        builder.Entity<IdentityRole>(b => { b.ToTable("Roles"); });
        builder.Entity<IdentityUserRole<string>>(b => { b.ToTable("UsuariosRoles"); });
        builder.Entity<IdentityUserClaim<string>>(b => { b.ToTable("UsuariosClaims"); });
        builder.Entity<IdentityUserLogin<string>>(b => { b.ToTable("UsuariosLogins"); });
        builder.Entity<IdentityRoleClaim<string>>(b => { b.ToTable("RolesClaims"); });
        builder.Entity<IdentityUserToken<string>>(b => { b.ToTable("UsuariosTokens"); });
    }

}
