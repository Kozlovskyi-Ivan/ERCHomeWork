using ERCHomeWork.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERCHomeWork.Models
{
    public class CarsContext :DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public CarsContext (DbContextOptions<CarsContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
