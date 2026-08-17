using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.RoomDtos
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string Amenities { get; set; } = null!;
        public string RoomStatus { get; set; } = null!;
    }
}
