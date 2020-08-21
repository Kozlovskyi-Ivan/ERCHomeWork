using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ERCHomeWork.Models.Entities
{
    public class Car
    {
        [Key]
        public string Number { get; set; }
        public int Mileage { get; set; }//to do
        //[Column(TypeName ="datatime2")]
        public DateTime Data { get; set; }
        public CarModel CarModel { get; set; }
    }
}
