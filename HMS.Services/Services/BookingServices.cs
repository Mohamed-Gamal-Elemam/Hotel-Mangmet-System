using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.BookingModule;
using HMS.Core.Entities.RoomModule;
using HMS.Core.Entities.SecurityModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTOs.BookingDtos;
using Shared.Message;
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
        private readonly IMapper _mapper;
        private readonly IEmailServices _email;

        public BookingServices( IUnitOfWork unitOfWork ,ILogger<BookingServices> logger,IMapper mapper ,IEmailServices email)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _email = email;
        }

        public async Task<GenaricResponse<bool>> CanselBookingAsync(Guid BookingId)
        {
            var genaricResponse = new GenaricResponse<bool>();
            try
            {
                var Booking = await _unitOfWork.GetRepository<Booking, Guid>().GetByIdAsync(BookingId, null, [B => B.HotelUser]);
                if (Booking is null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "Booking Not Found To Cancelled";
                    return genaricResponse;
                }
                if (Booking.Status == BookingStatus.Paid)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "Can Not Cancelled this Booking Becous is Already Paid";
                    return genaricResponse;
                }
                Booking.Status = BookingStatus.Cancelled;
                Booking.UpdatedAt = DateTime.Now;
                _unitOfWork.GetRepository<Booking, Guid>().UpdateAsync(Booking);
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result)
                {
                    genaricResponse.StutesCode = StatusCodes.Status200OK;
                    genaricResponse.Message = "Seccess to Cancelled This Booking";
                    genaricResponse.Data = true;

                    var Email = new Email
                    {
                        To = Booking.HotelUser.Email!,
                        Subject = "CcYour Booking Has Been Cancelled ",
                        Body = "Sorry,we have cancel your booking becouse of emergance reasons please contact us for more details"
                    };
                    await _email.SendEmail(Email);

                }
                else
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Feaild to cancle this booking";
                }
                return genaricResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Feaild to cancle this booking");
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Feaild to cancle this booking";
                return genaricResponse ;
            }


        }

        public async Task<GenaricResponse<Guid>> CreateBookingAsync(string userId, CreateBookingDto createBooking)
        {
            var genaricResponse = new GenaricResponse<Guid>();
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
                    genaricResponse.Data = booking.Id;

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

        public async Task<GenaricResponse<IEnumerable<BookingDto>>> CreateBookingForAdminAsync()
        {
            var genaricResponse=new GenaricResponse<IEnumerable<BookingDto>>();
            var Bookings =await _unitOfWork.GetRepository<Booking, Guid>().GetAllAsync(null, null, x=>x.CreatedAt, [B=>B.HotelUser]);
            if (Bookings is null )
            {
                genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                genaricResponse.Message = "no Booking to show";
                return genaricResponse;
            }


            var mappedBooking=_mapper.Map<IEnumerable<BookingDto>>(Bookings);
            genaricResponse.StutesCode = StatusCodes.Status200OK;
            genaricResponse.Message = "Success to Retrive All Booking ";
            genaricResponse.Data = mappedBooking;
            return genaricResponse;


        }

        public async Task<GenaricResponse<IEnumerable<GuestBookingDto>>> GetAllBookingForGuestAsync(string GuestId)
        {
            var genaricResponse = new GenaricResponse<IEnumerable<GuestBookingDto>>();
            var Booking=await _unitOfWork.GetRepository<Booking,Guid>()
                .GetAllAsync(x=>x.HotelUserId == GuestId,null ,x=>x.CreatedAt);
            if (Booking is null ||!Booking.Any())
            {
                genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                genaricResponse.Message = "no Booking to show";
                return genaricResponse;

            }
            var MappedBooking=_mapper.Map<IEnumerable<GuestBookingDto>>(Booking);

            genaricResponse.StutesCode = StatusCodes.Status200OK;
            genaricResponse.Message = "Success to Retrive All Booking ";
            genaricResponse.Data = MappedBooking;
            return genaricResponse;


        }
    }
}
