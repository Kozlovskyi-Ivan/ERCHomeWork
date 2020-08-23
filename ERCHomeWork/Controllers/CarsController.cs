using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        }
        [Route("GetCars")]
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var cars = await (from item in context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand)
                       select new CarResult{
                           Number=item.Number ,
                           Data = item.Data.ToShortDateString(),
                           Mileage = item.Mileage, 
                           Model=item.CarModel.Model, 
                           Brand=item.CarModel.Brand.Name}).ToListAsync();
            return Ok(cars);
        }
        [Route("GetFromExcel")]
        [HttpGet]
        public async Task<ActionResult> GetFromExcel()
        {
            if (context.CarModels.Any() == false)
            {
                ExcelTo excelTo = new ExcelTo(context);
                excelTo.GetExcel();
                return Ok("Add Models from Excel");
            }
            else
            {
                return BadRequest("Models exist");
            }
        }
        [Route("GenerateCars")]
        [HttpGet]
        public async Task<ActionResult> GenerateCars()
        {
            if (!(context.CarModels.Any()))
            {
                return BadRequest("Error: Need add Models");
            }
            else if((context.Cars.Any()))
            {
                return BadRequest("Cars exist");
            }
            else
            {
                GenerateCars generate = new GenerateCars(context);
                generate.Generate();
                return Ok("Add Cars");
            }
        }
        [Route("GetCars/{id}")]
        [HttpGet]
        public async Task<ActionResult> GetBySort(string id)
        {
            var cars = await (from item in context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand)
                              orderby id
                              select new CarResult
                              {
                                  Number = item.Number,
                                  Data = item.Data.ToShortDateString(),
                                  Mileage = item.Mileage,
                                  Model = item.CarModel.Model,
                                  Brand = item.CarModel.Brand.Name
                              }).ToListAsync();
            switch (id)
            {
                case("Number"):
                    { cars = cars.OrderBy((x) => x.Number).ToList(); }
                    break;
                case ("Mileage"):
                    { cars=cars.OrderBy((x) => x.Mileage).ToList(); }
                    break;
                case ("Data"):
                    { cars = cars.OrderBy((x) => Convert.ToDateTime(x.Data)).ToList(); }
                    break;
                case ("Model"):
                    { cars = cars.OrderBy((x) => x.Model).ToList(); }
                    break;
                case ("Brand"):
                    { cars = cars.OrderBy((x) => x.Brand).ToList(); }
                    break;
                default:
                    break;
            }
            return Ok(cars);
        }

        [HttpGet("Car/{id}")]
        public async Task<ActionResult> Get([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var car= await context.Cars.Include(x=>x.CarModel).ThenInclude(x=>x.Brand).FirstOrDefaultAsync((x) => x.Number == id);
            if (car!=null)
            {
                return Ok(new CarResult() { Number = car.Number, Mileage = car.Mileage, Data = car.Data.ToShortDateString(), Brand = car.CarModel.Brand.Name, Model = car.CarModel.Model });
            }
            return NotFound();
        }
        [Route("Brands")]
        [HttpGet]
        public async Task<ActionResult> GetBrands()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await context.Brands.ToListAsync());
        }        
        [Route("Models/{brand}")]
        [HttpGet("{brand}")]
        public async Task<ActionResult> GetModels(string brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(await context.CarModels.Where(x=>x.Brand.Name==brand).ToListAsync());
        }

        [Route("AddCar")]
        [HttpPost]
        public async Task<ActionResult> AddCar(CarResult car)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (null!=context.Cars.FirstOrDefault(x=>x.Number==car.Number))
            {
                return BadRequest("This Car exist");
            }
            var carS = new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) };
            if (carS != null)
            {
                await context.Cars.AddAsync(new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) });
                await context.SaveChangesAsync();
                return Ok($"Add car {car.Number}");
            }
            return BadRequest("No car"); 
        }

        [Route("UpdateCar")]
        //[HttpPut]
        [HttpPut]
        public async Task<ActionResult> Put(CarUpdate car)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var carFind=await context.Cars.FirstOrDefaultAsync((x) => x.Number == car.OldNumber);
            if (carFind != null)
            {
                context.Cars.Remove(await context.Cars.FirstOrDefaultAsync(x => x.Number == car.OldNumber));
                await context.Cars.AddAsync(new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) });
                await context.SaveChangesAsync();
                return Ok($"Update car {carFind.Number}");

            }
            return BadRequest("No car");
        }
        [Route("Delete/{id}")]
        [HttpDelete]
        public async Task<ActionResult> Delete(string id)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            context.Cars.Remove(await context.Cars.FirstOrDefaultAsync(x => x.Number == id));
            await context.SaveChangesAsync();
            return Ok($"Delete car {id}");
        }

    }
}