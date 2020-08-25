using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ERCHomeWork.Models.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        public string Number { get; set; }
        public int Mileage { get; set; }//to do
                                        //[Column(TypeName ="datatime2")]
                                        //[Display(Name = "Date")]
                                        //[DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
                                        //[DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Data { get; set; }
        public CarModel CarModel { get; set; }
    }
}
