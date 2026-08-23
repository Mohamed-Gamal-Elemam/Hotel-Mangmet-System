using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.BookingDtos;
using Shared.Resposne;
using System.Security.Claims;

namespace HMS.API.Controllers
{

    public class BookingsController : BaseApiController
    {
        private readonly IBookingServices _bookingServices;

        public BookingsController(IBookingServices bookingServices)
        {
            _bookingServices = bookingServices;
        }
        [Authorize(Roles =("Guest"))]
        [HttpPost]
        public async Task<ActionResult<GenaricResponse<string>>> CreateBooking([FromBody] CreateBookingDto createBooking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Response = await _bookingServices.CreateBookingAsync(userId!, createBooking);
            return HandelResponse(Response);
        }
    }
}
