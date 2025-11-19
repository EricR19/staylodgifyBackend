using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using BookingSite.Domain.Entities;
using Microsoft.Extensions.Configuration;


namespace BookingSite.Infrastructure.Context
{
    public partial class BookingDbContext : DbContext
    {
        public BookingDbContext() { }

        public BookingDbContext(DbContextOptions<BookingDbContext> options)
            : base(options) { }

        public virtual DbSet<Availability> Availabilities { get; set; }
        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<Receipts> Receipts { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<User> Users { get; set; }

      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Availability>(entity =>
            {
                entity.ToTable("availability");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.room_id).HasColumnName("room_id");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.is_available).HasColumnName("is_available").HasDefaultValueSql("'1'");
                entity.Property(e => e.Price).HasColumnName("price").HasPrecision(10, 2);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Availability)
                    .HasForeignKey(e => e.room_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("availability_ibfk_1");
            });

            modelBuilder.Entity<Guest>(entity =>
            {
                entity.ToTable("guests");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.TenantId).HasColumnName("tenant_id"); 
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Logs>(entity =>
            {
                entity.ToTable("logs");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.User_Id).HasColumnName("user_id");
                entity.Property(e => e.Tenant_Id).HasColumnName("tenant_id");
                entity.Property(e => e.Action).HasColumnName("action");
                entity.Property(e => e.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.logs)
                    .HasForeignKey(e => e.Tenant_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("logs_ibfk_2");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Logs)
                    .HasForeignKey(e => e.User_Id)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("logs_ibfk_1");
            });

            modelBuilder.Entity<Payments>(entity =>
            {
                entity.ToTable("payments");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Tenant_Id).HasColumnName("tenant_id");
                entity.Property(e => e.Reservation_Id).HasColumnName("reservation_id");
                entity.Property(e => e.Payment_Type).HasColumnName("payment_type");
                entity.Property(e => e.Provider).HasColumnName("provider");
                entity.Property(e => e.Provider).HasColumnName("provider_payment_id");
                entity.Property(e => e.Card_Last4).HasColumnName("card_last4");
                entity.Property(e => e.Card_Brand).HasColumnName("card_brand");
                entity.Property(e => e.Receipt_Url).HasColumnName("receipt_url");
                entity.Property(e => e.Amount).HasColumnName("amount").HasPrecision(10, 2);
                entity.Property(e => e.Currency).HasColumnName("currency").HasDefaultValueSql("'USD'");
                entity.Property(e => e.Status).HasColumnName("status").HasDefaultValueSql("'pending'");
                entity.Property(e => e.Paid_At).HasColumnName("paid_at");
                entity.Property(e => e.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.Payments)
                    .HasForeignKey(e => e.Reservation_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("payments_ibfk_2");

                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.payments)
                    .HasForeignKey(e => e.Tenant_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("payments_ibfk_1");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("properties");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Tenant_Id).HasColumnName("tenant_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.Main_Image).HasColumnName("main_image");
                entity.Property(e => e.Other_Images).HasColumnName("other_images");
                entity.Property(e => e.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.properties)
                    .HasForeignKey(e => e.Tenant_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("properties_ibfk_1");
            });

            modelBuilder.Entity<Receipts>(entity =>
            {
                entity.ToTable("receipts");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Reservation_Id).HasColumnName("reservation_id");
                entity.Property(e => e.File_Url).HasColumnName("file_url");
                entity.Property(e => e.Uploaded_At).HasColumnName("uploaded_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.Receipts)
                    .HasForeignKey(e => e.Reservation_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("receipts_ibfk_1");
            });

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.ToTable("reservations");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Tenant_Id).HasColumnName("tenant_id");
                entity.Property(e => e.Room_Id).HasColumnName("room_id");
                entity.Property(e => e.Guest_Id).HasColumnName("guest_id");
                entity.Property(e => e.Start_Date).HasColumnName("start_date");
                entity.Property(e => e.End_Date).HasColumnName("end_date");
                entity.Property(e => e.Status).HasColumnName("status").HasDefaultValueSql("'pending'");
                entity.Property(e => e.Created_At).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Guest)
                    .WithMany(g => g.Reservations)
                    .HasForeignKey(e => e.Guest_Id)
                    .HasConstraintName("reservations_ibfk_3");

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Reservations)
                    .HasForeignKey(e => e.Room_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("reservations_ibfk_2");

                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.reservations)
                    .HasForeignKey(e => e.Tenant_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("reservations_ibfk_1");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("rooms");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.property_id).HasColumnName("property_id");
                entity.Property(e => e.room_number).HasColumnName("room_number");
                entity.Property(e => e.description).HasColumnName("description");
                entity.Property(e => e.capacity).HasColumnName("capacity");
                entity.Property(e => e.price_per_night).HasColumnName("price_per_night").HasPrecision(10, 2);
                entity.Property(e => e.images).HasColumnName("images");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Property)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(e => e.property_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("rooms_ibfk_1");
            });

            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.ToTable("tenants");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Contact_email).HasColumnName("contact_email");
                entity.Property(e => e.Plan).HasColumnName("plan");
                entity.Property(e => e.Status).HasColumnName("status").HasDefaultValueSql("'active'");
                entity.Property(e => e.Subscription_expires_at).HasColumnName("subscription_expires_at");
                entity.Property(e => e.Created_at).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Tenant_id).HasColumnName("tenant_id");
                entity.Property(e => e.name).HasColumnName("name");
                entity.Property(e => e.email).HasColumnName("email");
                entity.Property(e => e.password_hash).HasColumnName("password_hash");
                entity.Property(e => e.Role).HasColumnName("role");
                entity.Property(e => e.created_at).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Tenant)
                    .WithMany(t => t.users)
                    .HasForeignKey(e => e.Tenant_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("users_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}