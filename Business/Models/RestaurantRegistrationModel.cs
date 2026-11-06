using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class RestaurantRegistrationModel
{
    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(250)]
    public string Address { get; set; } = string.Empty;

    [Required, Phone]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(60)]
    public string Cuisine { get; set; } = string.Empty;

    public List<OpeningHour> OpeningHours { get; set; } = [];

    [Range(1, 1000)]
    public int TableCapacity { get; set; }

    public List<string> PhotoNames { get; set; } = [];

    [Required, StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public OwnerAccount Owner { get; set; } = new();

    public class OpeningHour
    {
        public DayOfWeek Day { get; set; }
        public bool IsClosed { get; set; }
        public TimeOnly? Open { get; set; }
        public TimeOnly? Close { get; set; }
    }

    public class OwnerAccount
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;
    }
}
