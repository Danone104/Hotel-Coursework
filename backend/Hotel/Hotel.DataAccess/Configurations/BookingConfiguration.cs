using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Configurations
{
    public class BookingConfiguration: IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).ValueGeneratedNever();
            builder.Property(b => b.Status)
                .HasConversion<string>();


            builder
                .HasOne<Client>()
                .WithMany()
                .HasForeignKey(b => b.ClientId);
            builder
                .HasOne<Room>()
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
