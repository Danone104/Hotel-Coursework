using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hotel.DataAccess.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.ComplexProperty(e => e.TelephoneNum, p =>
            {
                p.IsRequired();
                p.Property(n => n.Value).HasColumnName("PhoneNumber");
            });
            builder.ComplexProperty(e => e.Name, fn =>
            {
                fn.IsRequired();
                fn.Property(n => n.FirstName).HasColumnName("FirstName");
                fn.Property(n => n.LastName).HasColumnName("LastName");
                fn.Property(n => n.Patronymic).HasColumnName("Patronymic");
            });
            builder.HasOne<User>()
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId);
        }
    }
}