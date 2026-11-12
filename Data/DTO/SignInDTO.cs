using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO;

public class SignInDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
