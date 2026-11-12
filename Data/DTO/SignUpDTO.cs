using Data.VO;

namespace Data.DTO;

public class SignUpDTO
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public UserVO.UserType Type { get; set; } = UserVO.UserType.Client;
}
