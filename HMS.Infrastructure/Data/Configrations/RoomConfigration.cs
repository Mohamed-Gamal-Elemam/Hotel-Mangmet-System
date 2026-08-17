using HMS.Core.Entities.RoomModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Infrastructure.Data.Configrations
{
    public class RoomConfigration : BaseConfigration<Room, int>, IEntityTypeConfiguration<Room>
    {
        public new void Configure(EntityTypeBuilder<Room> builder)
        {
            base.Configure(builder);
            builder.Property(r => r.Id)
                .UseIdentityColumn(100, 1);
            builder.Property(r => r.Description)
                .HasMaxLength(150);
            builder.Property(r => r.PricePerNight)
                .IsRequired()
                .HasPrecision(18, 2);


        }
    }
}
