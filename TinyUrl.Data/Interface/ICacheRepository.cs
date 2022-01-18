using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.Data.Interface
{
    public interface ICacheRepository
    {
        Task<string> Get(string key);
        Task Set(string key, string value);
        Task<bool> SetWithExpirationTime(string key, string value, TimeSpan expiration);
        Task<bool> ScriptEvaluate(string script, string key, string value);
    }
}
