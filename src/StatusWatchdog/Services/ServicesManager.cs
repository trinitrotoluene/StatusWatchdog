using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using StatusWatchdog.Models;
using StatusWatchdog.Services.Entities;

namespace StatusWatchdog.Services
{
    public class ServicesManager
    {
        private const string ROOT_VIEW_CACHE_KEY = "_servicesManager_root_view_cache";

        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _config;

        public ServicesManager(IMemoryCache memoryCache, IConfiguration config)
        {
            _memoryCache = memoryCache;

            _config = config;
        }

        public Service CreateService(Service service)
        {
            var model = new ServiceModel(service);

            using var ctx = CreateContext();

            ctx.Services.Add(model);
            ctx.SaveChanges();

            return new Service(model);
        }

        public Service GetService(int id)
        {
            using var ctx = CreateContext();

            var service = ctx.Services.Include(x => x.Incidents).FirstOrDefault(x => x.Id == id);
            if (service == null)
                return null;
            return new Service(service);
        }

        public Service GetService(string slug)
        {
            using var ctx = CreateContext();

            var service = ctx.Services.AsEnumerable().FirstOrDefault(x => x.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
            if (service == null)
                return null;
            return new Service(service);
        }

        public IEnumerable<Service> GetServices()
        {
            using var ctx = CreateContext();
            return ctx.Services.ToList().Select(x => new Service(x));
        }

        public Service Update(Service updatedService)
        {
            var model = new ServiceModel(updatedService);

            using var ctx = CreateContext();
            ctx.Services.Update(model);
            ctx.SaveChanges();

            return new Service(model);
        }

        public void DeleteService(int id)
        {
            using var ctx = CreateContext();
            ctx.Services.Remove(new ServiceModel { Id = id });
            ctx.SaveChanges();
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}
