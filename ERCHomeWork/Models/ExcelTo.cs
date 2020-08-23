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
        public void Test()
        {
            //New instance of ExcelEngine is created 
            //Equivalent to launching Microsoft Excel with no workbooks open
            //Instantiate the spreadsheet creation engine
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.              
            //string basePath = _hostingEnvironment.WebRootPath + @"\XlsIO\Sample.xlsx";
            string basePath = @"\XlsIO\Sample.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);

            //Access first worksheet from the workbook.
            IWorksheet worksheet = workbook.Worksheets[0];

            //Set Text in cell A3.
            worksheet.Range["A3"].Text = "Hello World";

            //Defining the ContentType for excel file.
            string ContentType = "Application/msexcel";

            //Define the file name.
            string fileName = "Output.xlsx";

            //Creating stream object.
            MemoryStream stream = new MemoryStream();

            //Saving the workbook to stream in XLSX format
            workbook.SaveAs(stream);

            stream.Position = 0;

            //Closing the workbook.
            workbook.Close();

            //Dispose the Excel engine
            excelEngine.Dispose();

            //Creates a FileContentResult object by using the file contents, content type, and file name.
            //return File(stream, ContentType, fileName);

        }
        public void GetExcel()
        {
            //using (var client = new WebClient())
            //{
            //    client.DownloadFile("https://drive.google.com/file/d/1tVyBibhr4Yf045PCh71c9W1SyDdGmm-3/view?usp=sharing", "Cars2.xlsx");
            //}
            CarsContext.Cars.RemoveRange(CarsContext.Cars);
            CarsContext.CarModels.RemoveRange(CarsContext.CarModels);

            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;
            string basePath = (@"./Models/Cars.xlsx");
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            List<CarModel> carModels = new List<CarModel>();
            foreach (var item in workbook.Worksheets)
            {
                if (item.Index != 0)
                {
                    //list.Add(item.Name);
                    Brand brand = new Brand() { Name = item.Name };
                    foreach (var item2 in item.Columns[0].Cells)
                    {
                        if (item.Name != item2.Value)
                        {
                            carModels.Add(new CarModel { Model = item2.Value, Brand = brand });
                        }
                        //Models.Add(item2.Value);
                    }
                }
            }
            CarsContext.CarModels.AddRange(carModels);
            CarsContext.SaveChanges();
            workbook.Close();
            excelEngine.Dispose();

        }
        public void GetWebExcel()
        {
            using (var client = new WebClient())
            {
                //File.Delete(@"./Models/Cars.xlsx");
                client.UseDefaultCredentials = true;
                client.DownloadFile("https://drive.google.com/uc?export=download&id=1tVyBibhr4Yf045PCh71c9W1SyDdGmm-3", @"./Models/Cars.xlsx");
                WaitForFile(@"./Models/Cars.xlsx");
            }
            CarsContext.Cars.RemoveRange(CarsContext.Cars);
            CarsContext.CarModels.RemoveRange(CarsContext.CarModels);

            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Xlsx;
            string basePath = (@"./Models/Cars.xlsx");
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            List<CarModel> carModels = new List<CarModel>();
            foreach (var item in workbook.Worksheets)
            {
                if (item.Index != 0)
                {
                    //list.Add(item.Name);
                    Brand brand = new Brand() { Name = item.Name };
                    foreach (var item2 in item.Columns[0].Cells)
                    {
                        if (item.Name != item2.Value)
                        {
                            carModels.Add(new CarModel { Model = item2.Value, Brand = brand });
                        }
                        //Models.Add(item2.Value);
                    }
                }
            }
            CarsContext.CarModels.AddRange(carModels);
            CarsContext.SaveChanges();
            workbook.Close();
            excelEngine.Dispose();

        }
        public static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static void WaitForFile(string filename)
        {
            //This will lock the execution until the file is ready
            //TODO: Add some logic to make it async and cancelable
            while (!IsFileReady(filename)) { }
        }
    }
}
