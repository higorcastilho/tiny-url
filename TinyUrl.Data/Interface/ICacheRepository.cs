using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrl.Data.Interface
{
    public interface ICacheRepository
    {
        string Get(string key);
        void Set(string key, string value);
        bool SetWithExpirationTime(string key, string value, TimeSpan expiration);
        bool ScriptEvaluate(string script, string key, string value);
    }
}
