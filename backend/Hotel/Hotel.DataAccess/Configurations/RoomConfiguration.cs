using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r=> r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever();
            builder.Property(r => r.Status)
                .HasConversion<string>();
        }
    }
}