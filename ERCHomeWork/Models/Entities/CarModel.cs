using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERCHomeWork.Models.Entities
{
    public class CarModel
    {
        public int CarModelId { get; set; }
        public string Model { get; set; }
        public Brand Brand { get; set; }
    }
}
