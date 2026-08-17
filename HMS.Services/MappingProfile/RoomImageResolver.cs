using AutoMapper;
using AutoMapper.Execution;
using HMS.Core.Entities.RoomModule;
using Microsoft.Extensions.Configuration;
using Shared.DTOs.RoomDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Services.MappingProfile
{
    public class RoomImageResolver : IValueResolver<Room, RoomDetailsDto, List<string>>
    {
        private readonly IConfiguration _configuration;

        public RoomImageResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> Resolve(Room source, RoomDetailsDto destination, List<string> destMember, ResolutionContext context)
        {
            return source.RoomImages.Select(ri => $"{_configuration["URLs:BaseURL"]}/{ri.ImageUrl}").ToList();
        }
    }
}
