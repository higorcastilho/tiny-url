using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyUrl.Domain.Entity;

namespace TinyUrl.Data.Interface
{
    public interface ILinkRepository
    {
        Task<Link> GetByShortUrl(string shortUrl);
        void Create(Link link);
    }
}
