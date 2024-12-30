using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Shoes_store.Models;

public partial class ShoppingDbContext : DbContext
{
    public ShoppingDbContext()
    {
    }

    public ShoppingDbContext(DbContextOptions<ShoppingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=HOME-PC\\SQLEXPRESS;Initial Catalog=ShoppingDB;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.IdBasket).HasName("PK__Basket__FFA73A6361BACAA5");
            entity.ToTable("Basket");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Baskets)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Basket__IdProduc__5165187F");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Baskets)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Basket__IdUser__52593CB8");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.IdBrand).HasName("PK__Brands__662A6659FEBAE792");
            entity.Property(e => e.BrandName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);

            
            entity.HasMany(b => b.Products)
                  .WithOne(p => p.Brand)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.IdFavorites).HasName("PK__Favorite__085BD2A2CD61BE60");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Favorites__IdPro__5535A963");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Favorites__IdUse__5629CD9C");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("PK__Products__2E8946D499085657");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Size)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.Brand)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.IdBrand)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Products__IdBran__4E88ABD4");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.IdReview).HasName("PK__Reviews__BB56047DE8FCDFAD");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Reviews__IdProdu__59063A47");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reviews_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__Users__B7C926383C188E54");
            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534FB4BE0ED").IsUnique();
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
