using HMS.Core.Entities.BookingModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configrations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CheckInDate)
                .IsRequired();

            builder.Property(x => x.CheckOutDate)
                .IsRequired();

            builder.Property(x => x.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("EGP")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.PayMobOrderId)
                .HasMaxLength(100);

            builder.Property(x => x.PayMobPaymentKey)
                .HasMaxLength(500);

            builder.Property(x => x.HotelUserId)
                .IsRequired();

            // Room -> Bookings
            builder.HasOne(x => x.Room)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // HotelUser -> Bookings
            builder.HasOne(x => x.HotelUser)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.HotelUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
