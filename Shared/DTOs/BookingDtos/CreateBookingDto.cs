using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.BookingDtos
{
    public class CreateBookingDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
    }
}
