using HMS.Core.Entities.RoomModule;
using Microsoft.AspNetCore.Http;
using Shared.DTOs.RoomDtos;
using Shared.QueryPramters;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IRoomService
    {
        public Task<GenaricResponse<IEnumerable<RoomDto>>> GetAllRoomsForGestAsync(string?roomType,string?sort);
        public Task<GenaricResponse<RoomDetailsDto>> GetRoomByIdAsync(int id);
        public Task<GenaricResponse<IEnumerable<RoomForAdminDto>>> GetRoomsForAdminOrStaff(RoomQueryParams queryParams);
        public Task<GenaricResponse<bool>> CreateRoomAsync(CreateRoomDto createRoom);
        public Task<GenaricResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDto updateRoom);
        public Task<GenaricResponse<bool>> DeleteRoomAsync(int id);
        public Task<GenaricResponse<bool>> UploadRoomImagesAsync(int roomId, List<IFormFile> files);
        public Task<GenaricResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId);
    }
}
