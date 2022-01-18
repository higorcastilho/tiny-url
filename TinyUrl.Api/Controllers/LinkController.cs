using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TinyUrl.Api.Models;
using TinyUrl.Service.Interface;

namespace TinyUrl.Api.Controllers
{
    [ApiController]
    public class LinkController : Controller
    {
        private readonly ILinkService _service;
        public LinkController(ILinkService service)
        {
            _service = service;
        }
        [Route("")]
        [HttpGet("{shortUrl}")]
        public async Task<ActionResult> GetLongUrlRedirect(string shortUrl)
        {
            try
            {
                var longUrl = await _service.GetShortenedUrlRedirect(shortUrl);
                return Redirect(longUrl);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error: {e.Message} - {e.InnerException?.Message}");
            }
        }
        [Route("Link")]
        [HttpPost]
        public async Task<ActionResult<string>> Generate([FromBody] LongUrl longUrl)
        {
            try
            {
                var host = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var generatedLink = await _service.Generate(longUrl.Url, host);
                
                var name = Dns.GetHostName(); // get container id
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                
                return Ok(generatedLink);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error: {e.Message} - {e.InnerException?.Message}");
            }
        }
    }
}
