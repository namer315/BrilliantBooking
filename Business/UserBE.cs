using Data.DAO;
using Data.DTO;
using Data.Exceptions;
using Data.Localization;
using Data.Managers;
using Data.VO;
using Data.VO.Restaurant;

namespace Business;

public class UserBE
{
    private UserDAO _dao = new UserDAO();

    public async Task<UserVO> SignInAsync(SignInDTO signIn)
    {
        if (string.IsNullOrWhiteSpace(signIn.Email) || string.IsNullOrWhiteSpace(signIn.Password))
            throw new AppException(ErrorCode.FieldIsRequired, $"{Resources.Email} , {Resources.Password}");
        // Encrypt password once and reuse the value
        string passwordEncrypt = DataEncryptorBE.EncryptString(signIn.Password);
        UserVO user = await new UserDAO().GetUserBy(signIn.Email , passwordEncrypt);
        if (user is null)
            throw new Exception("The email or password you entered is incorrect. Please check your details and try again.");

        UserSessionManager.LogIn(user, signIn);

        return user;
    }

    public async Task<UserVO> SignUpAsync(SignUpDTO signUp)
    {
        Validation(signUp);
        UserVO user = CreateUserBy(signUp);
        // TODO: Replace stub with real account creation.
        return await _dao.Merge(user);
    }

    private UserVO CreateUserBy(SignUpDTO signUp)
    {
        UserVO user = GetNew();

        user.Name = signUp.FullName;
        user.Email = signUp.Email;
        user.Phone = signUp.Phone;
        user.Password = DataEncryptorBE.EncryptString(signUp.Password);

        if(signUp.Type == UserVO.UserType.Owner)
        {
            user.Owner = new RestaurantOwnerVO();
            user.Owner.Name = signUp.FullName;
            user.Owner.Email = signUp.Email;
            user.Owner.Phone = signUp.Phone;
        }

        return user;
    }

    private UserVO GetNew()
    {
        UserVO user = new UserVO();

        return user;
    }

    private void Validation(SignUpDTO signUp)
    {
        if (string.IsNullOrWhiteSpace(signUp.FullName))
            throw new AppException(ErrorCode.FieldIsRequired , Resources.Name);

        if (string.IsNullOrWhiteSpace(signUp.Email))
            throw new AppException(ErrorCode.FieldIsRequired , nameof(signUp.Email));

        if (string.IsNullOrWhiteSpace(signUp.Password))
            throw new AppException(ErrorCode.FieldIsRequired , nameof(signUp.Password));

        if (string.IsNullOrWhiteSpace(signUp.ConfirmPassword))
            throw new AppException(ErrorCode.FieldIsRequired , nameof(signUp.ConfirmPassword));

        if (signUp.Password != signUp.ConfirmPassword)
            throw new AppException(ErrorCode.InvalidUser);

        if (signUp.Type == UserVO.UserType.Management)
            throw new AppException(ErrorCode.AccessDenied);
    }
}
