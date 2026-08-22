using HMS.Core.Entities.RoomModule;
using HMS.Core.Entities.SecurityModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Entities.BookingModule
{
    public class Booking :BaseEntity<Guid>
    {
        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "EGP";

        public BookingStatus Status { get; set; }

        public string? PayMobOrderId { get; set; }

        public string? PayMobPaymentKey { get; set; }

        public DateTime? PaidDate { get; set; }

        // Foreign Keys
        public int RoomId { get; set; }

        public string HotelUserId { get; set; } = null!;

        // Navigation Properties
        public Room Room { get; set; } = null!;

        public HotelUser HotelUser { get; set; } = null!;

    }
}
