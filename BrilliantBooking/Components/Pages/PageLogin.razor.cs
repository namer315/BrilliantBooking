using Data.DTO;
using FDM;

namespace BrilliantBooking.Components.Pages;

public partial class PageLogin
{
    private UserFDM _fdm = new UserFDM();
    private SignInDTO _signIn = new();
    private SignUpDTO _signUp = new();
    private bool _isSignIn = true;
    private bool _isBusy;
    private string? _errorMessage;
    private bool _showPassword;
    private bool _showConfirmPassword;
    private string _diningPreference = "main";

    private string PasswordInputType => _showPassword ? "text" : "password";

    private string ConfirmPasswordInputType => _showConfirmPassword ? "text" : "password";

    private bool PasswordsMismatch =>
        !string.IsNullOrEmpty(_signUp.ConfirmPassword) && _signUp.Password != _signUp.ConfirmPassword;

    private int PasswordStrength
    {
        get
        {
            var password = _signUp.Password ?? string.Empty;
            if (password.Length == 0)
            {
                return 0;
            }

            var score = 0;
            if (password.Length >= 8) score++;
            if (password.Any(char.IsUpper) && password.Any(char.IsLower)) score++;
            if (password.Any(char.IsDigit)) score++;
            if (password.Any(c => !char.IsLetterOrDigit(c))) score++;
            return score;
        }
    }

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
    }

    private void ToggleConfirmPasswordVisibility()
    {
        _showConfirmPassword = !_showConfirmPassword;
    }

    private void ShowSocialPlaceholder(string provider)
    {
        ShowMessage($"{provider} sign-in is not available yet.", "Coming soon");
    }

    private void ShowForgotPassword()
    {
        ShowMessage("Password reset instructions will be sent to your email.", "Forgot password");
    }

    private void ShowSignIn()
    {
        _isSignIn = true;
        _errorMessage = null;
    }

    private void ShowSignUp()
    {
        _isSignIn = false;
        _errorMessage = null;
    }

    private async Task HandleSignInAsync()
    {
        _errorMessage = null;
        await RunAsync(async () =>
        {
            if (await _fdm.SignInAsync(_signIn))
            {
                ShowMessage("Signed in successfully.", "Welcome");
            }
        });
    }

    private async Task HandleSignUpAsync()
    {
        _errorMessage = null;
        if (PasswordsMismatch)
        {
            _errorMessage = "Passwords do not match.";
            ShowWarning(_errorMessage);
            return;
        }

        await RunAsync(async () =>
        {
            if (await _fdm.SignUpAsync(_signUp))
            {
                ShowMessage("Account created successfully.", "Welcome");
            }
        });
    }

    private async Task RunAsync(Func<Task> action)
    {
        _isBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ShowException(ex);
        }
        finally
        {
            _isBusy = false;
        }
    }

}