using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoscowWeather.Models
{
    public class MoscowWeatherContext : DbContext
    {
        public MoscowWeatherContext (DbContextOptions<MoscowWeatherContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<MoscowWeather.Models.DBWeather> DBWeather { get; set; }
    }
}
