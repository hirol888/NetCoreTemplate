using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetCoreTemplate.Domain.Entities;

namespace NetCoreTemplate.Persistence.Configurations {
  public class UserConfiguration : IEntityTypeConfiguration<User> {
    public void Configure(EntityTypeBuilder<User> builder) {
      builder.HasKey(e => e.Id);

      builder.Property(e => e.Id)
        .ValueGeneratedNever();

      builder.Property(e => e.FirstName).HasMaxLength(50);
    }
  }
}
