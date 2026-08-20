using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Infrastructure.ExternalServices
{
    public class EmailSetting
    {
        public string Email { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public int Port { get; set; }
        public string Password { get; set; } = null!;
    }
}
