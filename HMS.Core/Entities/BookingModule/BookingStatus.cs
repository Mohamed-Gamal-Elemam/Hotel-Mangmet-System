using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Entities.BookingModule
{
    public enum BookingStatus
    {
        PendingPayment,
        Paid,
        Cancelled,
        Failed
    }
}
