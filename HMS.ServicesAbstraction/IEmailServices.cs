using Shared.Message;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.ServicesAbstraction
{
    public interface IEmailServices
    {
        Task SendEmail(Email email);
    }
}
