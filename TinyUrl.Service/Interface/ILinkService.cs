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
        string Generate(string longUrl, string host);
        string GetShortenedUrlRedirect(string shortUrl);
    }
}
