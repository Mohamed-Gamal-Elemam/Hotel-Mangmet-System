using HMS.Core.Contracts;
using HMS.Core.Entities.BookingModule;
using HMS.Core.Entities.RoomModule;
using HMS.Core.Entities.SecurityModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTOs.BookingDtos;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace HMS.Services.Services
{
    public class BookingServices : IBookingServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingServices> _logger;

        public BookingServices( IUnitOfWork unitOfWork ,ILogger<BookingServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<GenaricResponse<string>> CreateBookingAsync(string userId, CreateBookingDto createBooking)
        {
            var genaricResponse = new GenaricResponse<string>();
            try
            {
                if (createBooking == null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                    genaricResponse.Message = "no data to booking";
                    return genaricResponse;

                }
                if (createBooking.CheckInDate < DateTime.Now || createBooking.CheckInDate > createBooking.CheckOutDate)
                {
                    genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                    genaricResponse.Message = "this date is not vaild to Booking";
                    return genaricResponse;

                }
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(createBooking.RoomId, null, [r => r.Bookings]);

                if (room == null || room.RoomStatus == RoomStatus.NotExist || room.RoomStatus == RoomStatus.Maintenance)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "This Room is not Available to Booking";
                    return genaricResponse;
                }
                var HasConfilect = room.Bookings.Any(B => (B.Status == BookingStatus.PendingPayment || B.Status == BookingStatus.Paid) &&
                (B.CheckInDate < createBooking.CheckOutDate) &&
                (B.CheckOutDate > createBooking.CheckInDate));
                if (HasConfilect)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "This Room is not Available to Booking in this Dates";
                    return genaricResponse;
                }

                var nights = (createBooking.CheckOutDate - createBooking.CheckInDate).Days;
                var totalAmount = room.PricePerNight * nights;
              var booking = new Booking()
            {
                CheckInDate = createBooking.CheckInDate,
                CheckOutDate = createBooking.CheckOutDate,
                CreatedAt = DateTime.Now,
                HotelUserId = userId,
                Id = Guid.NewGuid(),
                RoomId = room.Id,
                TotalAmount = totalAmount,
            };
                await _unitOfWork.GetRepository<Booking, Guid>().AddAsync(booking);
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result)
                {
                    genaricResponse.StutesCode = StatusCodes.Status200OK;
                    genaricResponse.Message = "Seccess to create Booking";
                    genaricResponse.Data = booking.Id.ToString();

                }
                else
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Falid create Booking";

                }
                return genaricResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falid create Booking");
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Falid create Booking";
                return genaricResponse;
            }

           
            


        }
    }
}
