using AutoMapper;
using HMS.Core.Entities.RoomModule;
using Shared.DTOs.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Services.MappingProfile
{
    internal class RoomProfile :Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomDto>();
            CreateMap<Room, RoomDetailsDto>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom<RoomImageResolver>());
            CreateMap<Room, RoomForAdminDto>();
            CreateMap<CreateRoomDto, Room>();
            CreateMap<UpdateRoomDto, Room>();
        }
    }

}
