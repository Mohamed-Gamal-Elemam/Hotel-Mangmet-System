using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.RoomDtos
{
    public class RoomDetailsDto
    {
        public string RoomType { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public string Description { get; set; } = null!;
        public string Amenities { get; set; } = null!;
        public string RoomStatus { get; set; } = null!;
        public List<string> ImageUrls { get; set; } = [];
    }
}
