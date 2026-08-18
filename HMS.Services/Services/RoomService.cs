using AutoMapper;
using HMS.Core.Contracts;
using HMS.Core.Entities.RoomModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.DTOs.RoomDtos;
using Shared.QueryPramters;
using Shared.Resposne;
using System.Linq.Expressions;

namespace HMS.Services.Services
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IAttachementService _attachmentService;

        public RoomService(IUnitOfWork unitOfWork,IMapper mapper ,ILogger<RoomService> logger ,IAttachementService attachmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachmentService = attachmentService;
        }

        public async Task<GenaricResponse<bool>> CreateRoomAsync(CreateRoomDto createRoom)
        {
            var GenaricResponse = new GenaricResponse<bool>();

            try
            {
                if (createRoom is null)
                {
                    GenaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                    GenaricResponse.Message = "Invalid room data";
                    GenaricResponse.Data = false;
                    return GenaricResponse;
                }
                var room = _mapper.Map<Room>(createRoom);
                await _unitOfWork.GetRepository<Room, int>().AddAsync(room);

                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    GenaricResponse.StutesCode = StatusCodes.Status201Created;
                    GenaricResponse.Message = "Room created successfully";
                    GenaricResponse.Data = true;
                }
                else
                {
                    GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    GenaricResponse.Message = "Failed to create room";
                    GenaricResponse.Data = false;
                }
                return GenaricResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An Error occurred while creating a room ");
                GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                GenaricResponse.Message = "An error occurred while creating the room";
                GenaricResponse.Data = false;
                return GenaricResponse;
            }
        }

        public async Task<GenaricResponse<bool>> DeleteRoomAsync(int id)
        {
            var GenaricResponse = new GenaricResponse<bool>();
            try
            {
                //uncomplete logic
                var room =await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(id);
                if (room ==null)
                {
                    GenaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    GenaricResponse.Message = "Room not found";
                    GenaricResponse.Data = false;
                    return GenaricResponse;
                }

                room.RoomStatus = RoomStatus.NotExist;
                 _unitOfWork.GetRepository<Room, int>().UpdateAsync(room);
                room.UpdatedAt = DateTime.UtcNow;
                var result = await _unitOfWork.SaveChangesAsync()>0;
                if (result)
                {
                    GenaricResponse.StutesCode = StatusCodes.Status200OK;
                    GenaricResponse.Message = "Room deleted successfully";
                    GenaricResponse.Data = true;
                }
                else
                {
                    GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    GenaricResponse.Message = "Failed to delete room";
                    GenaricResponse.Data = false;
                }
                return GenaricResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An Error occurred while deleting a room ");
                GenaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                GenaricResponse.Message = "An error occurred while deleting the room";
                GenaricResponse.Data = false;
                return GenaricResponse;
            }

        }

        public async Task<GenaricResponse<bool>> DeleteRoomImageAsync(int roomId, int imageId)
        {
            var genaricResponse = new GenaricResponse<bool>();
            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId, null, [R => R.RoomImages]);
                if (room is null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "Room not found";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                if (room.RoomImages.Count == 0 || !room.RoomImages.Any())
                {
                    genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                    genaricResponse.Message = "No images found for this room";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                var image = room.RoomImages.FirstOrDefault(I => I.Id == imageId);
                if (image is null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "Image not found";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                _unitOfWork.GetRepository<RoomImages, int>().DeleteAsync(image);
                var IsDeltedFromServer = _attachmentService.DeleteFileAsync(image.ImageUrl,"Rooms");
                if (!IsDeltedFromServer)
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Failed to delete image from server";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result)
                {
                    genaricResponse.StutesCode = StatusCodes.Status200OK;
                    genaricResponse.Message = "Image deleted successfully";
                    genaricResponse.Data = true;
                }
                else
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Failed to delete image";
                    genaricResponse.Data = false;
                }
                return genaricResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "An error occurred while deleting the image");
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "An error occurred while deleting the image";
                genaricResponse.Data = false;
                return genaricResponse;
            }
        }

        public async Task<GenaricResponse<IEnumerable<RoomDto>>> GetAllRoomsForGestAsync(string? roomType, string? sort)
        {
            Enum.TryParse<RoomType>(roomType, out var roomTypeEnum);
            var GenaricResponse = new GenaricResponse<IEnumerable<RoomDto>>();
            Expression<Func<Room, bool>> filter = R => (roomType ==null|| R.RoomType == roomTypeEnum )&&(R.RoomStatus==RoomStatus.Available||R.RoomStatus==RoomStatus.Reserved); 
            Expression<Func<Room, object>>? orderBy = null;
            Expression<Func<Room, object>>? orderByDesc = null;

            if(sort != null)
            {
                switch (sort.ToLower())
                {
                    case "priceasc":
                        orderBy = R => R.PricePerNight;
                        break;
                    case "pricedesc":
                        orderByDesc = R => R.PricePerNight;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                orderBy = R => R.Id;
            }

            var rooms =await _unitOfWork.GetRepository<Room, int>().GetAllAsync(filter, orderBy, orderByDesc);
            if (rooms is null || !rooms.Any())
            {
                GenaricResponse.StutesCode = StatusCodes.Status404NotFound;
                GenaricResponse.Message = "No rooms found";
                return GenaricResponse;

            }
            else
            {
                GenaricResponse.StutesCode = StatusCodes.Status200OK;
                GenaricResponse.Message = "Rooms retrieved successfully";
                GenaricResponse.Data = _mapper.Map<IEnumerable<RoomDto>>(rooms);
            }
            return GenaricResponse;

        }

        public async Task<GenaricResponse<RoomDetailsDto>> GetRoomByIdAsync(int RoomId)
        {
            var GenaricResponse = new GenaricResponse<RoomDetailsDto>();
            Expression<Func<Room, bool>> filter = R=>(R.RoomStatus == RoomStatus.Available || R.RoomStatus == RoomStatus.Reserved);
            var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync  (RoomId, filter, [R=>R.RoomImages]);
            if (room is null)
            {
                GenaricResponse.StutesCode = StatusCodes.Status404NotFound;
                GenaricResponse.Message = "Room not found";
                return GenaricResponse;
            }
            else
            {
                GenaricResponse.StutesCode = StatusCodes.Status200OK;
                GenaricResponse.Message = "Room retrieved successfully";
                GenaricResponse.Data = _mapper.Map<RoomDetailsDto>(room);
                return GenaricResponse;
            }
        }

        public async Task<GenaricResponse<IEnumerable<RoomForAdminDto>>> GetRoomsForAdminOrStaff(RoomQueryParams queryParams)
        {
          
            var genaricResponse = new GenaricResponse<IEnumerable<RoomForAdminDto>>();
            Enum.TryParse<RoomType>(queryParams.roomType, out var roomTpeEnum);
            Expression<Func<Room,bool>> filter = R=>(queryParams.roomType ==null ||R.RoomType== roomTpeEnum)&& 
            (queryParams.roomStatuse==null ||R.RoomStatus== Enum.Parse<RoomStatus>(queryParams.roomStatuse));
            Expression<Func<Room, object>>? orderByExc = null;
            Expression<Func<Room, object>>? orderByDescExc = null;
            if (queryParams.Sort != null)
            {
                switch (queryParams.Sort.ToLower())
                {
                    case "priceasc":
                        orderByExc = R => R.PricePerNight;
                        break;
                    case "pricedesc":
                        orderByDescExc = R => R.PricePerNight;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                orderByDescExc = R => R.CreatedAt;
            }
            var rooms =await _unitOfWork.GetRepository<Room, int>().GetAllAsync(filter, orderByExc, orderByDescExc);
            if (rooms is null || !rooms.Any())
            {
                genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                genaricResponse.Message = "No rooms found";
                return genaricResponse;
            }
            else
            {
                genaricResponse.StutesCode = StatusCodes.Status200OK;
                genaricResponse.Message = "Rooms retrieved successfully";
                genaricResponse.Data = _mapper.Map<IEnumerable<RoomForAdminDto>>(rooms);
                return genaricResponse;
            }

        }

        public async Task<GenaricResponse<bool>> UpdateRoomAsync(int id, UpdateRoomDto updateRoom)
        {
            var genaricResponse = new GenaricResponse<bool>();
            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(id);
                if (room is null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                    genaricResponse.Message = "Room not found";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                var updatedRoom = _mapper.Map(updateRoom, room);
                room.UpdatedAt = DateTime.UtcNow;
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    genaricResponse.StutesCode = StatusCodes.Status200OK;
                    genaricResponse.Message = "Room updated successfully";
                    genaricResponse.Data = true;
                }
                else
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Failed to update room";
                    genaricResponse.Data = false;
                }
                return genaricResponse;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed to update room");
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Failed to update room";
                genaricResponse.Data = false;
                return genaricResponse;
            }

        }

        public async Task<GenaricResponse<bool>> UploadRoomImagesAsync(int roomId, List<IFormFile> files)
        {
            var genaricResponse = new GenaricResponse<bool>();
            try
            {
                var room = await _unitOfWork.GetRepository<Room, int>().GetByIdAsync(roomId);
                if (room is null)
                {
                    genaricResponse.StutesCode = StatusCodes.Status400BadRequest;
                    genaricResponse.Message = "Room not found";
                    genaricResponse.Data = false;
                    return genaricResponse;
                }
                foreach (var file in files)
                {
                    var fileName = await _attachmentService.UploadFileAsync(file, "Rooms");
                    if (fileName is null)
                        continue;
                    var roomImage = new RoomImages
                    {
                        ImageUrl = fileName,
                        RoomId = room.Id
                    };
                    await _unitOfWork.GetRepository<RoomImages, int>().AddAsync(roomImage);
                }
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result)
                {
                    genaricResponse.StutesCode = StatusCodes.Status200OK;
                    genaricResponse.Message = "Images uploaded successfully";
                    genaricResponse.Data = true;
                }
                else
                {
                    genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                    genaricResponse.Message = "Failed to upload images";
                    genaricResponse.Data = false;
                }
                return genaricResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading images");
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "An error occurred while uploading images";
                genaricResponse.Data = false;
                return genaricResponse;

            }
        }
    }
}
