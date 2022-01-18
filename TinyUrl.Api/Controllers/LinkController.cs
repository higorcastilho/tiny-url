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
    //[Route("[controller]")]
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
        public ActionResult GetLongUrlRedirect(string shortUrl)
        {
            var longUrl = _service.GetLongUrlRedirect(shortUrl);
            return Redirect(longUrl);
        }
        [Route("Link")]
        [HttpPost]
        public async Task<ActionResult> Generate([FromBody] LongUrl longUrl)
        {
            try
            {
                var link = await _service.Generate(longUrl.Url);
                
                var name = Dns.GetHostName(); // get container id
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                
                return Ok(link);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error: {e.Message} - {e.InnerException?.Message}");
            }
        }
    }
}
