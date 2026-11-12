using Business;
using Data.DTO;

namespace FDM;

public class UserFDM
{
    private UserBE _be = new();

    public async Task<bool> SignInAsync(SignInDTO signIn)
        => await _be.SignInAsync(signIn);

    public async Task<bool> SignUpAsync(SignUpDTO signUp)
        => await _be.SignUpAsync(signUp);
}
