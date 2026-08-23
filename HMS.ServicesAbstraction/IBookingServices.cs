using HMS.Core.Entities.SecurityModule;
using Shared.DTOs.BookingDtos;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IBookingServices
    {
        Task<GenaricResponse<string>> CreateBookingAsync(string userId, CreateBookingDto createBooking); 
    }
}
