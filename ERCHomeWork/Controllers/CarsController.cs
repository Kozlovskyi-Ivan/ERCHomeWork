using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ERCHomeWork.Models;
using ERCHomeWork.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;


namespace ERCHomeWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        CarsContext context;
        public CarsController(CarsContext context)
        {
            this.context = context;
        }
        //[Route("GetCars")]
        //[HttpGet]
        //public async Task<ActionResult> Get()
        //{
        //    var cars = await (from item in context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand)
        //                      select new CarResult
        //                      {
        //                          Number = item.Number,
        //                          Data = item.Data.ToShortDateString(),
        //                          Mileage = item.Mileage,
        //                          Model = item.CarModel.Model,
        //                          Brand = item.CarModel.Brand.Name
        //                      }).ToListAsync();
        //    return Ok(cars);
        //}
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
            //else if ((context.Cars.Any()))
            //{
            //    return BadRequest("Cars exist");
            //}
            else
            {
                GenerateCars generate = new GenerateCars(context);
                generate.Generate();
                return Ok("Add Cars");
            }
        }
        [Route("GetCarsCount")]
        [HttpGet]
        public async Task<ActionResult> GetCarsNumber()
        {
            if (!(context.CarModels.Any()))
            {
                return BadRequest("No car");
            }
            else
            {
                return Ok(await context.Cars.CountAsync());
            }
        }
        [Route("GetCars/{sortType}/{page}")]
        [HttpGet]
        public async Task<ActionResult> GetBySort(string sortType, int page)
        {
            //var cars = await (from item in context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand)
            //                  orderby id
            //                  select new CarResult
            //                  {
            //                      Number = item.Number,
            //                      Data = item.Data.ToShortDateString(),
            //                      Mileage = item.Mileage,
            //                      Model = item.CarModel.Model,
            //                      Brand = item.CarModel.Brand.Name
            //                  }).ToListAsync();
            var cars = await context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand).ToListAsync();
            switch (sortType)
            {
                case ("Number"):
                    { cars = cars.OrderBy((x) => x.Number).Skip(page*10).Take(10).ToList(); }
                    break;
                case ("Mileage"):
                    { cars = cars.OrderBy((x) => x.Mileage).Skip(page * 10).Take(10).ToList(); }
                    break;
                case ("Data"):
                    { cars = cars.OrderBy((x) => Convert.ToDateTime(x.Data)).Skip(page * 10).Take(10).ToList(); }
                    break;
                case ("Model"):
                    { cars = cars.OrderBy((x) => x.CarModel.Model).Skip(page * 10).Take(10).ToList(); }
                    break;
                case ("Brand"):
                    { cars = cars.OrderBy((x) => x.CarModel.Brand.Name).Skip(page * 10).Take(10).ToList(); }
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
            var car = await context.Cars.Include(x => x.CarModel).ThenInclude(x => x.Brand).FirstOrDefaultAsync((x) => x.Number == id);
            if (car != null)
            {
                return Ok(car);
                //return Ok(new CarResult() { Number = car.Number, Mileage = car.Mileage, Data = car.Data.ToShortDateString(), Brand = car.CarModel.Brand.Name, Model = car.CarModel.Model });
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
            return Ok(await context.CarModels.Where(x => x.Brand.Name == brand).ToListAsync());
        }

        //[Route("AddCar")]
        //[HttpPost]
        //public async Task<ActionResult> AddCar(CarResult car)

        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (null!=context.Cars.FirstOrDefault(x=>x.Number==car.Number))
        //    {
        //        return BadRequest("This Car exist");
        //    }
        //    var carS = new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) };
        //    if (carS != null)
        //    {
        //        await context.Cars.AddAsync(new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) });
        //        await context.SaveChangesAsync();
        //        return Ok($"Add car {car.Number}");
        //    }
        //    return BadRequest("No car"); 
        //}
        [Route("AddCar")]
        [HttpPost]
        public async Task<ActionResult> AddCar([FromBody] JsonElement json)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (null != context.Cars.FirstOrDefault(x => x.Number == json.GetProperty("Number").GetString()))
            {
                return BadRequest("This Car exist");
            }
            var car = new Car
            {
                Number = json.GetProperty("Number").GetString(),
                Mileage = json.GetProperty("Mileage").GetInt32(),
                Data = Convert.ToDateTime(json.GetProperty("Date").GetString()),
                CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == json.GetProperty("Model").GetString())
            };
            await context.Cars.AddAsync(car);
            await context.SaveChangesAsync();
            return Ok($"Add car {car.Number}");
        }


        //[Route("UpdateCar")]
        ////[HttpPut]
        //[HttpPost]
        //public async Task<ActionResult> Put(CarUpdate car)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var carFind = await context.Cars.FirstOrDefaultAsync((x) => x.Number == car.OldNumber);
        //    if (carFind != null)
        //    {
        //        context.Cars.Remove(await context.Cars.FirstOrDefaultAsync(x => x.Number == car.OldNumber));
        //        await context.Cars.AddAsync(new Car { Number = car.Number, Mileage = car.Mileage, Data = Convert.ToDateTime(car.Data), CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == car.Model) });
        //        await context.SaveChangesAsync();
        //        return Ok($"Update car {carFind.Number}");

        //    }
        //    return BadRequest("No car");
        //}
        [Route("UpdateCar")]
        [HttpPost]
        public async Task<ActionResult> UpdateCar([FromBody] JsonElement json)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var carFind = await context.Cars.FirstOrDefaultAsync((x) => x.Id == json.GetProperty("Id").GetInt32());
            if (carFind != null)
            {
                carFind.Number = json.GetProperty("Number").GetString();
                carFind.Mileage = json.GetProperty("Mileage").GetInt32();
                carFind.Data = Convert.ToDateTime(json.GetProperty("Date").GetString());
                carFind.CarModel = await context.CarModels.FirstOrDefaultAsync(x => x.Model == json.GetProperty("Model").GetString());
                await context.SaveChangesAsync();
                return Ok($"Update car {carFind.Number}");
            };
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