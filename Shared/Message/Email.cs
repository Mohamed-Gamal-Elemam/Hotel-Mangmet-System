using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Message
{
    public class Email
    {
        public string To { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
