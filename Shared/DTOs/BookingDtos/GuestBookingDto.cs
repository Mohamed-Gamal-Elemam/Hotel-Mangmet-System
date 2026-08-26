using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.BookingDtos
{
    public class GuestBookingDto
    {
        public int RoomId { get; set; }
        public string CheckInDate { get; set; } = null!;
        public string CheckOutDate { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal TotalAmount { get; set; }

    }
}
