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

    private string PasswordInputType => _showPassword ? "text" : "password";

    private void TogglePasswordVisibility()
    {
        _showPassword = !_showPassword;
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
        if (_signUp.Password != _signUp.ConfirmPassword)
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