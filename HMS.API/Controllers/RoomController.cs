using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.RoomDtos;
using Shared.QueryPramters;
using Shared.Resposne;

namespace HMS.API.Controllers
{
   
    public class RoomController : BaseApiController
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }
        [HttpGet("Public")]
        public async Task<ActionResult<GenaricResponse<IEnumerable<RoomDto>>>> GetAllRoomsForGestAsync (string? roomType, string? sort)
        {
           var response = await _roomService.GetAllRoomsForGestAsync(roomType, sort);
            return HandelResponse(response);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenaricResponse<RoomDetailsDto>>> GetRoomByIdAsync(int id)
        {
            var response = await _roomService.GetRoomByIdAsync(id);
            return HandelResponse(response);
        }
        [HttpGet("Admin")]
        public async Task<ActionResult<GenaricResponse<IEnumerable<RoomForAdminDto>>>> GetRoomsForAdminOrStuff([FromQuery] RoomQueryParams? queryParams)
        {
            var response = await _roomService.GetRoomsForAdminOrStaff(queryParams);
            return HandelResponse(response);
        }

        [HttpPost]
        public async Task<ActionResult<GenaricResponse<bool>>> CreateRoomAsync([FromBody] CreateRoomDto createRoom)
        {
            var response = await _roomService.CreateRoomAsync(createRoom);
            return HandelResponse(response);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<GenaricResponse<bool>>> UpdateRoomAsync(int id, [FromBody] UpdateRoomDto updateRoom)
        {
            var response = await _roomService.UpdateRoomAsync(id, updateRoom);
            return HandelResponse(response);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<GenaricResponse<bool>>> DeleteRoomAsync(int id)
        {
            var response = await _roomService.DeleteRoomAsync(id);
            return HandelResponse(response);
        }
        [HttpPost("{roomId}/UploadImages")]
        public async Task<ActionResult<GenaricResponse<bool>>> UploadRoomImages(int roomId, [FromForm] List<IFormFile> files)
        {
            var response = await _roomService.UploadRoomImagesAsync(roomId, files);
            return HandelResponse(response);
        }
        [HttpDelete("{roomId}/DeleteImage/{imageId}")]
        public async Task<ActionResult<GenaricResponse<bool>>> DeleteRoomImageAsync(int roomId, int imageId)
        {
            var response = await _roomService.DeleteRoomImageAsync(roomId, imageId);
            return HandelResponse(response);
        }
    }
}
