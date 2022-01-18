using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TinyUrl.Api.Models;

namespace TinyUrl.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LinkController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> Generate([FromBody] LongUrl longUrl)
        {
            try
            {
                //await _service.Create(longUrl.Url);
                
                var name = Dns.GetHostName(); // get container id
                var ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                Console.WriteLine($"Host Name: { Environment.MachineName} \t {name}\t {ip}");
                
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error: {e.Message} - {e.InnerException?.Message}");
            }
        }
    }
}
