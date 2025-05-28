using Mnf_Portal.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnf_Portal.Core.Interfaces
{
    public interface IMnfContextRepo<T> : IGenericRepository<T> where T : BaseEntity { }
}
