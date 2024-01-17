using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Data.Configuration
{
    public class CategoryConfiguration: IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Categoria");

            builder.Property(e => e.Id)
                .HasColumnName("IdCategoria");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Nombre")
                .HasMaxLength(50)
                .IsUnicode(true);

            builder.Property(e => e.Color)
                .IsRequired()
                .HasColumnName("Color")
                .HasMaxLength(8)
                .IsUnicode(false);
        }
    }
}
