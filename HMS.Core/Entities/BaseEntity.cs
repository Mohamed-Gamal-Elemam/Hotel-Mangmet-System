using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Core.Entities
{
    public class BaseEntity <Tkey>
    {
        public Tkey Id { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
