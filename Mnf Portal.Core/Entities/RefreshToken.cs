using Mnf_Portal.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnf_Portal.Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime ExpDate { get; set; }
        public string userId { get; set; }

        public AppUser AppUser { get; set; }
    }
}
