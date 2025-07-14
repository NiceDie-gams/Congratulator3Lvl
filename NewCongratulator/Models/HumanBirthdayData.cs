using System.ComponentModel.DataAnnotations;

namespace NewCongratulator.Models
{
    public class HumanBirthdayData
    {
        public int Id { get; set; }
        public string fullName { get; set; } = "Григорий Лепс";
        public int age { get; set; }
        public DateOnly birthdayDate { get; set; }
        public string emailAddress { get; set; } = string.Empty;
        public string? imageUrl { get; set; } = string.Empty;
    }
}
