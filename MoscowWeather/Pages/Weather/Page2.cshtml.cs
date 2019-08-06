using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoscowWeather.Models;

namespace MoscowWeather.Pages.Weather.Entity
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int Month { get; set; } = 0;
        [BindProperty]
        public int Year { get; set; } = 0;
        public bool IsCorrectFilter { get; set; } = false;
        public string Message { get; set; } = "";

        public DateTime minDate = DateTime.MinValue;
        public DateTime maxDate = DateTime.MinValue;

        private readonly MoscowWeather.Models.MoscowWeatherContext _context;

        public IndexModel(MoscowWeather.Models.MoscowWeatherContext context)
        {
            _context = context;
        }

        public IList<DBWeather> DBWeather { get;set; }

        public async Task OnGetAsync()
        {
            DBWeather = await _context.DBWeather.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            IsCorrectFilter = false;
            if (Year > 0 && Year < 2077)
            {
                if (Month != 0)
                {
                    if (Month > 0 && Month < 13)
                    {
                        IsCorrectFilter = true;
                        //Year = (int)year;
                        //Month = (int)month;
                        Message = $"Фильтрация данных по месяцу {Month} и году {Year}";
                        minDate = DateTime.MinValue;
                        minDate = minDate.AddMonths(Month - 1);
                        minDate = minDate.AddYears(Year - 1);
                        maxDate = minDate.AddMonths(1);
                    }
                    else
                    {
                        Message = $"Mесяц задан некорректно: {Month}";
                    }
                }
                else
                {
                    IsCorrectFilter = true;
                    //Year = (int)year;
                    //Month = 0;
                    Message = $"Фильтрация данных только по году: {Year}";

                    minDate = DateTime.MinValue;
                    minDate = minDate.AddYears(Year - 1);
                    maxDate = minDate.AddYears(1);
                }
            }
            else
            {
                //IsCorrectFilter = false;
                //Year = 0;
                Message = "Фильтрация не осуществляется.";
            }

            if (IsCorrectFilter)
            {
                DBWeather = await _context.DBWeather.Where(p => p.DateTimeObservation >= minDate && p.DateTimeObservation < maxDate).ToListAsync();
            }
            else DBWeather = await _context.DBWeather.ToListAsync();


            return Page(); //RedirectToPage("./Index");
        }

    }
}
