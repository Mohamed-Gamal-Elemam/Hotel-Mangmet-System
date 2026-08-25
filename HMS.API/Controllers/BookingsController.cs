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
        private readonly IPaymentServices _paymentServices;

        public BookingsController(IBookingServices bookingServices ,IPaymentServices paymentServices)
        {
            _bookingServices = bookingServices;
            _paymentServices = paymentServices;
        }
        [Authorize(Roles =("Guest"))]
        [HttpPost]
        public async Task<ActionResult<GenaricResponse<Guid>>> CreateBooking([FromBody] CreateBookingDto createBooking)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var Response = await _bookingServices.CreateBookingAsync(userId!, createBooking);
            return HandelResponse(Response);
        }
        [HttpPost("{id}/pay")]

        public async Task<ActionResult<GenaricResponse<string>>> GetPaymentUrl(Guid id)
        {
            var response = await _paymentServices.CreatePaymentUrlAsync(id);
            return HandelResponse(response);
        }
    }
}
