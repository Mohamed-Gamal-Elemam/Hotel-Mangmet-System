using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.RoomDtos
{
    public class RoomForAdminDto :RoomDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
