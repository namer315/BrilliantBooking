using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO;

public class SignUpDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string Phone { get; set; }
    public string FullName { get; set; }
}
