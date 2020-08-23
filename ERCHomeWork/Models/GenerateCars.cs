using ERCHomeWork.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERCHomeWork.Models
{
    public class GenerateCars
    {
        private CarsContext context;
        public GenerateCars(CarsContext context)
        {
            this.context = context;
        }
        public void Generate()
        {
            Random random = new Random();
            List<Car> cars = new List<Car>();
            string charL= "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string charN = "0123456789";
            for (int i = 0; i < random.Next(6, 12); i++)
            {
                string tempN = "";
                tempN += charL[random.Next(charL.Length)];
                tempN += charL[random.Next(charL.Length)];
                tempN += charN[random.Next(charN.Length)];
                tempN += charN[random.Next(charN.Length)];
                tempN += charN[random.Next(charN.Length)];
                tempN += charN[random.Next(charN.Length)];
                tempN += charL[random.Next(charL.Length)];
                tempN += charL[random.Next(charL.Length)];
                if (null==cars.FirstOrDefault(x=>x.Number== tempN))
                {
                    DateTime start = new DateTime(1995, 1, 1);
                    int range = (DateTime.Today - start).Days;
                    start = start.AddDays(random.Next(range));
                    var models = context.CarModels;
                    Car car = new Car() { 
                        Number = tempN, 
                        Mileage = random.Next(1000), 
                        Data=start,
                        CarModel = context.CarModels.ToList().ElementAt(random.Next(context.CarModels.Count())),
                    };
                    cars.Add(car);
                }
            }
            context.AddRange(cars);
            context.SaveChanges();
        }
    }
}
