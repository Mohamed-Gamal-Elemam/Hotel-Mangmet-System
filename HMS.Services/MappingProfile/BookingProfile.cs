using AutoMapper;
using HMS.Core.Entities.BookingModule;
using Shared.DTOs.BookingDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Services.MappingProfile
{
    public class BookingProfile:Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(des=>des.GuestFullName,option=>option.MapFrom(Src=>Src.HotelUser.FullName))
                .ForMember(des=>des.GuestEmail,option=>option.MapFrom(Src=>Src.HotelUser.Email));
            CreateMap<Booking, GuestBookingDto>()
                .ForMember(des => des.CheckInDate, opt => opt.MapFrom(src => src.CheckInDate.ToShortDateString()))
                .ForMember(des => des.CheckOutDate, opt => opt.MapFrom(src => src.CheckOutDate.ToShortDateString()));
        }
    }
}
