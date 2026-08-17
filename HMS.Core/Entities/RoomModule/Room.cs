using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Entities.RoomModule
{
    public class Room : BaseEntity<int>
    {
        public string Description { get; set; }=null!;
        public RoomType RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; }= null!;
        public RoomStatus RoomStatus { get; set; }
        public ICollection<RoomImages> RoomImages { get; set; } = [];

    }
}
