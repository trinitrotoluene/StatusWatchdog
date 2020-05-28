using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class KeyValueManager
    {
        private readonly IConfiguration _config;
        
        public KeyValueManager(IConfiguration config)
        {
            _config = config;
        }

        public bool TrySetValue(string key, string value)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            using var ctx = CreateContext();
            try
            {
                ctx.Add(new KeyValueModel
                {
                    Key = key,
                    Value = value
                });
                ctx.SaveChanges();

                return true;
            }
            catch (DbUpdateException)
            {
                // Swallow duplicate key exception
            }
            return false;
        }

        public bool TryGetValue(string key, out string value)
        {
            using var ctx = CreateContext();
            value = ctx.KeyValueStore.FirstOrDefault(x => x.Key.Equals(key))?.Value;

            return value != null;
        }

        public void DeleteKey(string key)
        {
            using var ctx = CreateContext();
            ctx.Remove(new KeyValueModel { Key = key });
            ctx.SaveChanges();
        }

        public IDictionary<string, string> GetValues(string[] keys)
        {
            using var ctx = CreateContext();
            var keyValueEntries = ctx.KeyValueStore.Where(x => keys.Any(y => y.Equals(x.Key)));

            var dictionary = new Dictionary<string, string>();
            foreach (var kv in keyValueEntries)
                dictionary[kv.Key] = kv.Value;

            return dictionary;
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}