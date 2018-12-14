using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Extensions;

namespace NetCoreTemplate.Persistence {
  public class NetCoreTemplateDbContext : DbContext {
    public NetCoreTemplateDbContext(DbContextOptions<NetCoreTemplateDbContext> options) : base(options) {}

    public DbSet<User> User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(NetCoreTemplateDbContext).Assembly);
    }
  }
}
