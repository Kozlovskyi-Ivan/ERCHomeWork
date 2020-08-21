using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERCHomeWork.Models;
using ERCHomeWork.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ERCHomeWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        CarsContext  context;
        public CarsController(CarsContext  context)
        {
            this.context = context;
            if (context.CarModels.Any() == false)
            {
                Brand brand = new Brand() { Name = "mazda" };
                Brand brand2 = new Brand() { Name = "Honda" };
                CarModel model = new CarModel() { Model = "maz1", Brand = brand };
                CarModel model2 = new CarModel() { Model = "maz2", Brand = brand };
                CarModel model3 = new CarModel() { Model = "hon1", Brand = brand2 };
                Car car = new Car() { Number = "num1", Data = DateTime.UtcNow, Mileage = 12, CarModel = model };
                Car car2 = new Car() { Number = "num2", Data = DateTime.UtcNow, Mileage = 0, CarModel = model };
                Car car3 = new Car() { Number = "num3", Data = DateTime.UtcNow, Mileage = 1331, CarModel = model3 };
                Car car4 = new Car() { Number = "num4", Data = DateTime.UtcNow, Mileage = 123, CarModel = model2 };
                context.Cars.AddRange(car, car2, car3, car4);
                context.SaveChanges();

            }
        }
        //// GET: api/Cars
        //[Route("~/api/GetCars")]
        //[HttpGet]
        //public List<Car> Get()
        //{
        //    //return new string[] { "value1", "value2" };
        //    return context.Cars.Include(x=>x.CarModel).ThenInclude(x=>x.Brand).ToList();
        //}
        // GET: api/Cars
        [Route("/GetCars")]
        [HttpGet]
        public List<CarResult> Get()
        {
            //return new string[] { "value1", "value2" };
            var cars = (from item in context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand)
                       select new CarResult{
                           Number=item.Number ,
                           Data=item.Data, 
                           Mileage=item.Mileage, 
                           Model=item.CarModel.Model, 
                           Brand=item.CarModel.Brand.Name}).ToList();
            return cars;
            //return context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand).Select((x)=>new sa { }).ToList();
        }

        // GET: api/Cars/5
        [HttpGet("Car/{id}")]
        public Car Get(string id)
        {
            return context.Cars.FirstOrDefault((x) => x.Number == id);
        }
        // GET: api/Cars/5
        [Route("/Brands")]
        [HttpGet]
        public List<Brand> GetBrands()
        {
            return context.Brands.ToList();
        }        
        // GET: api/Cars/5
        [Route("/Models")]
        [HttpGet("{brand}")]
        public List<CarModel> GetModels(string brand)
        {
            return context.CarModels.Where(x=>x.Brand.Name==brand).ToList();
        }

        // POST: api/Cars
        [Route("~/api/AddCar")]
        [HttpPost]
        public IActionResult AddCar([FromBody] Car car)
        {
            context.Cars.Add(car);
            context.SaveChanges();
            return Ok($"Add car {car.Number}");
        }

        // PUT: api/Cars/5
        [Route("~/api/UpdateCar")]
        [HttpPut("{id}")]
        public IActionResult Put([FromBody] Car car)
        {
            context.Cars.Update(car);
            context.SaveChanges();
            return Ok($"Update car {car.Number}");

        }

        // DELETE: api/ApiWithActions/5
        [Route("~/api/Delete/{id}")]
        [HttpDelete]
        //[HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            context.Cars.Remove(context.Cars.FirstOrDefault(x => x.Number == id));
            context.SaveChanges();
            return Ok($"Delete car {id}");
        }

    }
}