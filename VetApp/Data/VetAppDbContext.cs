using Microsoft.EntityFrameworkCore;
using VetApp.Models;

namespace VetApp.Data;

public class VetAppDbContext : DbContext
{
    public VetAppDbContext(DbContextOptions<VetAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Capability> Capabilities { get; set; }

    public DbSet<Owner> Owners { get; set; }

    public DbSet<Patient> Patients { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Veterinarian> Veterinarians { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Capability>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();
        });

        modelBuilder.Entity<Owner>(entity =>
        {
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Owner)
                .HasForeignKey<Owner>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Owners_UserId");

            entity.HasIndex(e => e.UserId, "IX_Owners_UserId").IsUnique();
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.ChipNumber).HasMaxLength(15);
            entity.Property(e => e.Species).HasMaxLength(50);
            entity.Property(e => e.Breed).HasMaxLength(50);

            entity.HasOne(d => d.Veterinarian)
                .WithMany(p => p.Patients)
                .HasForeignKey(d => d.VeterinarianId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Patients_VeterinarianId");

            entity.HasOne(d => d.Owner)
                .WithMany(p => p.Patients)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Patients_OwnerId");

            entity.HasIndex(e => e.ChipNumber, "IX_Patients_ChipNumber").IsUnique();
            entity.HasIndex(e => e.VeterinarianId, "IX_Patients_VeterinarianId");
            entity.HasIndex(e => e.OwnerId, "IX_Patients_OwnerId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)
                .UsingEntity("RolesCapabilities", j =>
                {
                    j.HasIndex("CapabilitiesId")
                    .HasDatabaseName("IX_RolesCapabilities_CapabilityId");
                });

            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_RoleId");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");
            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();
        });

        modelBuilder.Entity<Veterinarian>(entity =>
        {
            entity.Property(e => e.Clinic).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

            entity.HasOne(d => d.User).WithOne(p => p.Veterinarian)
                .HasForeignKey<Veterinarian>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Veterinarians_UserId");

            entity.HasIndex(e => e.Clinic, "IX_Veterinarians_Clinic");
            entity.HasIndex(e => e.UserId, "IX_Veterinarians_UserId").IsUnique();
        });
    }
}