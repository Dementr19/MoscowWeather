using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace MoscowWeather.Models
{
    public class DBWeather
    {
        public int Id { get; set; }

        [Display(Name = "Дата Время")]
        [DataType(DataType.DateTime)]
        public DateTime DateTimeObservation { get; set; }

        [Display(Name = "Темпе- ратура воздуха, гр. Ц")]
        public float? AirTemp { get; set; }

        [Display(Name = "Относи- тельная влажность, %")]
        public float? Humidity { get; set; }

        [Display(Name = "Точка росы, гр. Ц")]
        public float? DewPoint { get; set; }

        [Display(Name = "Атмосфер- ное давление, мм рт.ст.")]
        public int? AtmPressure { get; set; }

        [Display(Name = "Направле- ние ветра")]
        public string WindDir { get; set; }

        [Display(Name = "Скорость ветра, м/с")]
        public int? WindSpeed { get; set; }

        [Display(Name = "Облачность, %")]
        public int? Cloudiness { get; set; }

        [Display(Name = "Нижняя граница облачности, м")]
        public int? HeightCloudiness { get; set; }

        [Display(Name = "Горизон- тальная видимость, км")]
        public int? HorizontVisibility { get; set; }

        [Display(Name = "Погодные явления")]
        public string WeatherEvent { get; set; }

        //public virtual ICollection<Book> Books { get; set; }

    }
}
