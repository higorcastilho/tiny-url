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
        Task<string> Generate(string longUrl, string host);
        Task<string> GetShortenedUrlRedirect(string shortUrl);
    }
}
