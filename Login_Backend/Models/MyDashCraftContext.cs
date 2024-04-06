using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Login_Backend.Models;

public partial class MyDashCraftContext : DbContext
{
    public MyDashCraftContext()
    {
    }

    public MyDashCraftContext(DbContextOptions<MyDashCraftContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Benutzer> Benutzers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=JUSTIN\\TESTSQLSERVER;Initial Catalog=MyDashCraft;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Benutzer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Benutzer__3214EC27BB09D591");

            entity.ToTable("Benutzer");

            entity.HasIndex(e => e.Email, "UQ__Benutzer__A9D1053469D1255A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Erstellungsdatum).HasColumnType("datetime");
            entity.Property(e => e.LetztesAnmeldedatum)
                .HasColumnType("datetime")
                .HasColumnName("Letztes_Anmeldedatum");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Passwort).HasMaxLength(255);
            entity.Property(e => e.Rolle).HasMaxLength(50);
            entity.Property(e => e.EmailPasswordHash).HasMaxLength(128);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
