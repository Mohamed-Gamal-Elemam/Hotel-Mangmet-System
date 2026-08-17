using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.RoomDtos
{
    public class UpdateRoomDto
    {
        public RoomType RoomType { get; set; }
        public string Description { get; set; }
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = null!;
        public RoomStatus RoomStatus { get; set; }

    }
}
