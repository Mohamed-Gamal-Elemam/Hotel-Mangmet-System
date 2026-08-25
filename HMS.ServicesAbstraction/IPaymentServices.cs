using Shared.Resposne;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IPaymentServices
    {
        Task<GenaricResponse<string>> CreatePaymentUrlAsync(Guid BookingId);
    }
}
