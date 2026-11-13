using Business;
using Data.DTO;
using Data.VO;

namespace FDM;

public class UserFDM
{
    private UserBE _be = new();

    public async Task<UserVO> SignInAsync(SignInDTO signIn)
        => await _be.SignInAsync(signIn);

    public async Task<UserVO> SignUpAsync(SignUpDTO signUp)
        => await _be.SignUpAsync(signUp);
}
