using Mnf_Portal.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnf_Portal.Core.Entities
{
    public class ContactUs : BaseEntity
    {
        public string Email { get; set; }
        public string Description { get; set; }
        public int Rate { get; set; }
        public MessageType Type { get; set; }
    }
}
