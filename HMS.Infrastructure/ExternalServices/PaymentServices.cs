using HMS.Core.Contracts;
using HMS.Core.Entities.BookingModule;
using HMS.ServicesAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace HMS.Infrastructure.ExternalServices
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PaymentServices(IUnitOfWork unitOfWork ,HttpClient httpClient,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<GenaricResponse<string>> CreatePaymentUrlAsync(Guid BookingId)
        {
            var genaricResponse =new GenaricResponse<string>();
            var Booking = await _unitOfWork.GetRepository<Booking, Guid>().GetByIdAsync(BookingId, null, [B => B.HotelUser]);
            if (Booking is null)
            {
                genaricResponse.StutesCode = StatusCodes.Status404NotFound;
                genaricResponse.Message = "Booking not found to pay";
                return genaricResponse;
            }
            //Get Auth Token
            var authToken =await GetAuthTokenAsync();

            //get orderId [Intint]
            var ordeerId = await GetOrderIdAsync(authToken,Booking.TotalAmount,Booking.Currency);

            if (ordeerId is null)
            {
                genaricResponse.StutesCode= StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Falid to create Intent on PayMob";
                return genaricResponse;

            }
            Booking.PayMobOrderId=ordeerId;

            //create PaymentKey
            var paymentKey=await GetPaymentKeyAsync(authToken,ordeerId,Booking.TotalAmount,Booking.Currency,Booking.HotelUser.Email!,Booking.HotelUser.FullName,Booking.HotelUser.PhoneNumber!);

            if (paymentKey is null)
            {
                genaricResponse.StutesCode= StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Falid to create payment on PayMob";
                return genaricResponse;

            }
            Booking.PayMobPaymentKey=paymentKey;
            _unitOfWork.GetRepository<Booking, Guid>().UpdateAsync(Booking);
            Booking.UpdatedAt=DateTime.Now;

            var result = await _unitOfWork.SaveChangesAsync() > 0;
            if (result)
            {
                genaricResponse.StutesCode = StatusCodes.Status200OK;
                genaricResponse.Message = "Seccess to create payment Url";
                genaricResponse.Data = $"{_configuration["Paymob:BaseUrl"]}/acceptance/iframes/{_configuration["Paymob:IFrameId"]}?payment_token{paymentKey}";

            }
            else
            {
                genaricResponse.StutesCode = StatusCodes.Status500InternalServerError;
                genaricResponse.Message = "Falid to create payment Link";

            }
                return genaricResponse;



        }

        private async Task<string> GetAuthTokenAsync()
        {
            var respose = await _httpClient.PostAsJsonAsync($"{_configuration["Paymob:BaseUrl"]}/auth/tokens",
                new { api_key = _configuration["Paymob:ApiKey"] });
            var json =await respose.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("token").GetString()!;
        }

        private async Task<string> GetOrderIdAsync(string AuthToken ,decimal Amount ,string Currency)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["Paymobe:BaseUrl"]}/ecommerce/orders", new
            {
                auth_token = AuthToken,
                delivery_needed = false,
                amount_cents = (int)(Amount * 100),
                currency= Currency,
                items=Array.Empty<object>()

            });
            var json =await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("Id").GetInt32().ToString();
        }


        private async Task<string> GetPaymentKeyAsync(string authToken,string orderId,decimal amount ,string curency ,string email,string fullName,string phoneNumber)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_configuration["Paymob:BaseUrl"]}/acceptance/payment_keys", new
            {
                auth_token = authToken,
                amount_cents=(int)(amount*100),
                curency= curency,
                order_id= orderId,
                expiration =3600,
                integration_id =int.Parse( _configuration["Paymob:IntegrationId"]!),
                billing_data = new
                {
                    email=email,
                    firest_name=fullName.Split(' ')[0],
                    last_name=fullName.Split(' ')[1],
                    phone_number=phoneNumber,
                    aportment="NA",
                    floor="NA",
                    street="NA",
                    building="Na",
                    city= "El menoufia",
                    country="EG",
                    state="El menoufia"

                }

            });

            var json=await response.Content.ReadFromJsonAsync<JsonElement>();
            return  json.GetProperty("token").GetString()!;
        }
    }
}
