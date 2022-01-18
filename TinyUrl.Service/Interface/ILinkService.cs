using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Domain.Entity;

namespace TinyUrl.Service.Interface
{
    public interface ILinkService
    {
        Task<Link> Generate(string longUrl);
    }
}
