using System.ComponentModel.DataAnnotations;
using Data.VO.Restaurant;

namespace Business.Services;

public interface IRestaurantRegistrationService
{
    RegistrationResult Register(RestaurantVO model);
}

public record RegistrationResult(bool Succeeded, IReadOnlyList<string> Errors)
{
    public static RegistrationResult Success() => new(true, []);
    public static RegistrationResult Failure(IEnumerable<string> errors) => new(false, errors.ToList());
}

public class RestaurantRegistrationService : IRestaurantRegistrationService
{
    public RegistrationResult Register(RestaurantVO model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        if (Validator.TryValidateObject(model, context, results, validateAllProperties: true))
        {
            return RegistrationResult.Success();
        }

        return RegistrationResult.Failure(results.Select(r => r.ErrorMessage ?? "Invalid registration data."));
    }
}
