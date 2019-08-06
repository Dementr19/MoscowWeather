using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using MoscowWeather.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;

namespace MoscowWeather.Pages.Weather
{
    public class Page3Model : PageModel
    {
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(IEnumerable<IFormFile>  uploads)
        {
            // Perform an initial check to catch FileUpload class attribute violations.
            if (!ModelState.IsValid)
            {
                return Page();
            }
            foreach (var file in uploads)
            {
                if (file != null)
                {

                    var filePath = Directory.GetCurrentDirectory() + "/Files/" + file.FileName;
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }


                    //Подключаемся к БД

                    var builder = new ConfigurationBuilder();
                    // установка пути к текущему каталогу
                    builder.SetBasePath(Directory.GetCurrentDirectory());
                    // получаем конфигурацию из файла appsettings.json
                    builder.AddJsonFile("appsettings.json");
                    // создаем конфигурацию
                    var config = builder.Build();
                    // получаем строку подключения
                    string connectionString = config.GetConnectionString("MoscowWeatherContext");

                    var optionsBuilder = new DbContextOptionsBuilder<MoscowWeatherContext>();
                    var options = optionsBuilder
                        .UseSqlServer(connectionString)
                        .Options;

                    using (MoscowWeatherContext db = new MoscowWeatherContext(options))
                    {
                        DBWeather DBWeather = new DBWeather();

                        // Парсим загруженный файл

                        //Книга Excel
                        IWorkbook workbook;

                        //filePath = Directory.GetCurrentDirectory() + "/Files/moskva_2010.xlsx";

                        //Открываем файл надо try
                        using (var fileStreamX = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            workbook = WorkbookFactory.Create(fileStreamX);
                        }

                        //Получаем и обрабатываем 12 листов книги
                        for (int i = 0; i < 12; i++)
                        {
                            ISheet sheet = workbook.GetSheetAt(i);

                            //удаляем прежние данные за тот же месяц 
                            var minDate = DateTime.Parse(sheet.GetRow(4).GetCell(0).StringCellValue + " " + sheet.GetRow(4).GetCell(1).StringCellValue);
                            var maxDate = DateTime.Parse(sheet.GetRow(sheet.LastRowNum).GetCell(0).StringCellValue + " " + sheet.GetRow(sheet.LastRowNum).GetCell(1).StringCellValue);
                            db.DBWeather.RemoveRange(db.DBWeather.Where(p => p.DateTimeObservation >= minDate && p.DateTimeObservation <= maxDate));

                            //запускаем цикл по строкам
                            for (int row = 4; row <= sheet.LastRowNum; row++)
                            {
                                //получаем строку
                                var currentRow = sheet.GetRow(row);
                                if (currentRow != null) //null когда строка содержит только пустые ячейки
                                {
                                    //получаем значение ячеек
                                    var DateTimeOservation = DateTime.Parse(currentRow.GetCell(0).StringCellValue
                                        + " " + currentRow.GetCell(1).StringCellValue);
                                    var crAirTemp = (float)currentRow.GetCell(2).NumericCellValue;
                                    var crHumidity = (float)currentRow.GetCell(3).NumericCellValue;
                                    var crDewPoint = (float)currentRow.GetCell(4).NumericCellValue;
                                    var crAtmPressure = (int)currentRow.GetCell(5).NumericCellValue;
                                    var crWindDir = currentRow.GetCell(6).StringCellValue;
                                    int? crWindSpeed;
                                    try
                                    {
                                        crWindSpeed = (int)currentRow.GetCell(7).NumericCellValue;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        crWindSpeed = null;
                                    }
                                    int? crCloudiness;
                                    try
                                    {
                                        crCloudiness = (int)currentRow.GetCell(8).NumericCellValue;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        crCloudiness = null;
                                    }

                                    int? crHeightCloudiness;
                                    try
                                    {
                                        crHeightCloudiness = (int)currentRow.GetCell(9).NumericCellValue;
                                    }
                                    catch
                                    {
                                        crHeightCloudiness = null;
                                    }

                                    int? crHorizontVisibility;
                                    try
                                    {
                                        crHorizontVisibility = (int)currentRow.GetCell(10).NumericCellValue;
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        crHorizontVisibility = null;
                                    }
                                    string crWeatherEvent;
                                    try
                                    {
                                        crWeatherEvent = currentRow.GetCell(11).StringCellValue;
                                    }
                                    catch (NullReferenceException)
                                    {
                                        crWeatherEvent = null;
                                    }
                                    //Добавление строки в БД
                                    db.DBWeather.Add(new DBWeather
                                    {
                                        DateTimeObservation = DateTimeOservation,
                                        AirTemp = crAirTemp,
                                        Humidity = crHumidity,
                                        DewPoint = crDewPoint,
                                        AtmPressure = crAtmPressure,
                                        WindDir = crWindDir,
                                        WindSpeed = crWindSpeed,
                                        Cloudiness = crCloudiness,
                                        HeightCloudiness = crHeightCloudiness,
                                        HorizontVisibility = crHorizontVisibility,
                                        WeatherEvent = crWeatherEvent
                                    });
                                    await db.SaveChangesAsync();
                                }
                            }
                        }
                    }
                }
            }
            return Page(); //RedirectToPage("./Index");
        }

    }

}