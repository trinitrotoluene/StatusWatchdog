using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class KeyValuesManager
    {
        private readonly IConfiguration _config;

        public KeyValuesManager(IConfiguration config)
        {
            _config = config;
        }

        public void SetValue(string key, string value)
        {
            if (value == null)
            {
                throw new ArgumentException(nameof(value));
            }

            using var ctx = CreateContext();
            var existing = ctx.KeyValueStore.FirstOrDefault(x => x.Key.Equals(key));
            if (existing == null)
            {
                ctx.KeyValueStore.Add(new KeyValueModel { Key = key, Value = value });
            }
            else
            {
                existing.Value = value;
                ctx.Update(existing);
            }
            ctx.SaveChanges();
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
            
            foreach (var key in keys)
                if (!dictionary.ContainsKey(key))
                    dictionary[key] = null;

            return dictionary;
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}