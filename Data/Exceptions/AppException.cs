using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Exceptions;

public class AppException : Exception
{
    public ErrorCode ErrorCode { get; }

    public AppException(ErrorCode errorCode)
        : base(GetMessage(errorCode))
    {
        ErrorCode = errorCode;
    }
    public AppException(ErrorCode errorCode , params object[] args)
        : base(string.Format(
            ExceptionResource.ResourceManager.GetString(errorCode.ToString())
            ?? errorCode.ToString() ,
            args))
    {
        ErrorCode = errorCode;
    }
    //public AppException(ErrorCode errorCode , Exception innerException , params object[] args)
    //    : base(FormatMessage(errorCode , args) , innerException)
    //{
    //    ErrorCode = errorCode;
    //}

    //private static string FormatMessage(ErrorCode errorCode , object[] args)
    //{
    //    string template =
    //        Resources.ResourceManager.GetString(errorCode.ToString())
    //        ?? errorCode.ToString();

    //    return args?.Length > 0
    //        ? string.Format(template , args)
    //        : template;
    //}

    private static string GetMessage(ErrorCode errorCode)
    {
        return ExceptionResource.ResourceManager.GetString(errorCode.ToString())
               ?? errorCode.ToString();
    }
}
/// <summary>
/// Identifies the error that an <see cref="AppException"/> represents.
/// Each value maps to a localized message in <c>ExceptionResource</c>.
/// </summary>
public enum ErrorCode
{
    InvalidUser,
    BookingNotFound,
    AccessDenied,
    /// <summary>
    /// Indicates that a required input field was left empty.
    /// The name of the missing field must be passed as the first argument
    /// to <see cref="AppException(ErrorCode, object[])"/>, for example:
    /// <c>new AppException(ErrorCode.FieldIsRequired, nameof(signUp.Email))</c>.
    /// The field name is inserted into the <c>{0}</c> placeholder of the localized message.
    /// </summary>
    FieldIsRequired
}
