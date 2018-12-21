﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetCoreTemplate.Persistence.Data;

namespace NetCoreTemplate.Persistence.Migrations
{
    [DbContext(typeof(NetCoreTemplateDbContext))]
    partial class NetCoreTemplateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("NetCoreTemplate.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<DateTime>("Expires");

                    b.Property<DateTime>("ModifiedAtUtc");

                    b.Property<string>("RemoteIpAddress");

                    b.Property<string>("Token");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshToken");
                });

            modelBuilder.Entity("NetCoreTemplate.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Active");

                    b.Property<DateTime>("CreatedAtUtc");

                    b.Property<bool?>("Deleted");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50);

                    b.Property<string>("IdentityId");

                    b.Property<string>("LastIpAddress");

                    b.Property<DateTime?>("LastLoginDateUtc");

                    b.Property<string>("LastName")
                        .HasMaxLength(50);

                    b.Property<string>("Mobile");

                    b.Property<DateTime>("ModifiedAtUtc");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("NetCoreTemplate.Domain.Entities.RefreshToken", b =>
                {
                    b.HasOne("NetCoreTemplate.Domain.Entities.User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}