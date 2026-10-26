using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Configurations
{
    public class ClientConfiguration: IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                .ValueGeneratedNever();
            builder.ComplexProperty(c => c.TelephoneNum, p =>
            {
                p.IsRequired();
                p.Property(n => n.Value).HasColumnName("PhoneNumber");
            });
            builder.ComplexProperty(c => c.Name, fn =>
            {
                fn.IsRequired();
                fn.Property(n => n.FirstName).HasColumnName("FirstName");
                fn.Property(n => n.LastName).HasColumnName("LastName");
                fn.Property(n => n.Patronymic).HasColumnName("Patronymic");
            });
            builder
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Client>(c => c.UserId)
                .IsRequired(false);
        }
    }
}
