using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.XlsIO;
using System.IO;
using Syncfusion.Drawing;
using System.Reflection;
using ERCHomeWork.Models.Entities;
using System.Net;

namespace ERCHomeWork.Models
{
    public class ExcelTo
    {
        CarsContext CarsContext;
        public ExcelTo(CarsContext carsContext)
        {
            CarsContext = carsContext;
        }
        public void GetExcel(bool WebLoad = false)
        {
            if (WebLoad)
            {
                using (var client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.DownloadFile("https://drive.google.com/uc?export=download&id=1tVyBibhr4Yf045PCh71c9W1SyDdGmm-3", @"./Models/Cars.xlsx");
                }
            }
            CarsContext.Cars.RemoveRange(CarsContext.Cars);
            CarsContext.CarModels.RemoveRange(CarsContext.CarModels);

            ExcelEngine excelEngine = new ExcelEngine();

            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2013;
            string basePath = (@"./Models/Cars.xlsx");
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            List<CarModel> carModels = new List<CarModel>();
            foreach (var item in workbook.Worksheets)
            {
                if (item.Index != 0)
                {
                    Brand brand = new Brand() { Name = item.Name };
                    foreach (var item2 in item.Columns[0].Cells)
                    {
                        if (item.Name != item2.Value)
                        {
                            carModels.Add(new CarModel { Model = item2.Value, Brand = brand });
                        }
                    }
                }
            }
            CarsContext.CarModels.AddRange(carModels);
            CarsContext.SaveChanges();
            workbook.Close();
            excelEngine.Dispose();

        }
        //public void GetWebExcel()
        //{
        //    using (var client = new WebClient())
        //    {
        //        client.UseDefaultCredentials = true;
        //        client.DownloadFile("https://drive.google.com/uc?export=download&id=1tVyBibhr4Yf045PCh71c9W1SyDdGmm-3", @"./Models/Cars.xlsx");
        //    }
        //    CarsContext.Cars.RemoveRange(CarsContext.Cars);
        //    CarsContext.CarModels.RemoveRange(CarsContext.CarModels);

        //    ExcelEngine excelEngine = new ExcelEngine();

        //    IApplication application = excelEngine.Excel;

        //    application.DefaultVersion = ExcelVersion.Xlsx;
        //    string basePath = (@"./Models/Cars.xlsx");
        //    FileStream sampleFile = new FileStream(basePath, FileMode.Open);

        //    IWorkbook workbook = application.Workbooks.Open(sampleFile);
        //    List<CarModel> carModels = new List<CarModel>();
        //    foreach (var item in workbook.Worksheets)
        //    {
        //        if (item.Index != 0)
        //        {
        //            //list.Add(item.Name);
        //            Brand brand = new Brand() { Name = item.Name };
        //            foreach (var item2 in item.Columns[0].Cells)
        //            {
        //                if (item.Name != item2.Value)
        //                {
        //                    carModels.Add(new CarModel { Model = item2.Value, Brand = brand });
        //                }
        //            }
        //        }
        //    }
        //    CarsContext.CarModels.AddRange(carModels);
        //    CarsContext.SaveChanges();
        //    workbook.Close();
        //    excelEngine.Dispose();

        //}
    }
}
