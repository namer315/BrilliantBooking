using Data.DTO;

namespace Business;

public class UserBE
{
    public async Task<bool> SignInAsync(SignInDTO signIn)
    {
        // TODO: Replace stub with real authentication.
        return await Task.FromResult(true);
    }

    public async Task<bool> SignUpAsync(SignUpDTO signUp)
    {
        // TODO: Replace stub with real account creation.
        return await Task.FromResult(true);
    }
}
