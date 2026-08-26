using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs.BookingDtos
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string GuestFullName { get; set; } = null!;
        public string GuestEmail { get; set; }=null!;
        public string Status { get; set; }=null!;
        public int RoomId { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
