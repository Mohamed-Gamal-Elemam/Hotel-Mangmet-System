using HMS.Core.Entities.RoomModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.Data.Configrations
{
    public class RoomImageConfigration :BaseConfigration<RoomImages,int>, IEntityTypeConfiguration<RoomImages>
    {
        public new void Configure(EntityTypeBuilder<RoomImages> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.ImageUrl).IsRequired().HasMaxLength(500);
        }
    }
}
