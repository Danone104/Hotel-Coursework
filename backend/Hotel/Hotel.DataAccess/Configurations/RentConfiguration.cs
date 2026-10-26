using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Configurations
{
    public class RentConfiguration : IEntityTypeConfiguration<RentRoom>
    {
        public void Configure(EntityTypeBuilder<RentRoom> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r=>r.Id)
                .ValueGeneratedNever();
            builder.Property(r => r.Status)
                .HasConversion<string>();

            builder.HasOne<Booking>()
                .WithOne()
                .HasForeignKey<RentRoom>(b=>b.BookId);
        }
    }
}
