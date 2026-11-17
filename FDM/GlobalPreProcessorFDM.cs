using Business;
using Data.DTO;
using Data.VO;
using System;
using System.Collections.Generic;
using System.Text;

namespace FDM;

public class GlobalPreProcessorFDM
{
    public async Task<UserVO> SignInAsync(SignInDTO signIn)
         => await new UserBE().SignInAsync(signIn);
}
