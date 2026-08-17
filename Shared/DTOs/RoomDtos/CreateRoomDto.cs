using Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs.RoomDtos
{
    public class CreateRoomDto
    {
        [Required(ErrorMessage = "Room Status is required.")]
        public RoomStatus RoomStatus { get; set; }
        [Required(ErrorMessage = "Price per night is required.")]
        public decimal PricePerNight { get; set; }
        [Required(ErrorMessage = "Amenities are required.")]
        public string Amenities { get; set; } = null!;
        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; } = null!;

    }
}
