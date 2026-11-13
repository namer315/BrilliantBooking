using Data.Common;
using Data.DTO;
using Data.VO;
using Microsoft.AspNetCore.Http;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;
using System.Text;
using System.Text.Json;
using System.Text.Json;


namespace Data.Managers;

public class UserSessionManager
{
    public static Dictionary<Guid , UserSession> UserSessionList { get; set; } = new Dictionary<Guid , UserSession>();

    private static IHttpContextAccessor _httpContext = new HttpContextAccessor();

    // Add this method to allow DI assignment
    public static void SetHttpContextAccessor(IHttpContextAccessor accessor)
    {
        if (accessor != null)
            _httpContext = accessor;
    }
    public static bool CheckDictionaryKey(Guid key)
    {
        return UserSessionList.ContainsKey(key);
    }
    public static Guid PublicKey
    {
        get { return Guid.Parse("11111111-1111-1111-1111-111111111111"); }
    }
    public static bool IskeyExist
    {
        get
        {
            return _httpContext?.HttpContext?.Items?.ContainsKey(NameKeys.UserSessionKey) ?? false;
        }
    }

    public static Guid CurrentKey
    {
        get
        {
            var ctx = _httpContext.HttpContext;
            if (ctx is null || _httpContext.HttpContext.Items.Count == 0) return Guid.Empty;

            if (ctx.Items.TryGetValue(NameKeys.UserSessionKey , out var item) && item is Guid guid)
                return guid;

            var value = ctx.Session?.GetString(NameKeys.UserSessionKey);
            return value is not null
                ? JsonSerializer.Deserialize<Guid>(value)
                : throw new InvalidOperationException("Current Online User Key cannot be found.");
        }
    }

    public static UserSession? CurrentUserSession
    {
        get
        {
            if (CurrentKey is Guid currentKey)
                return UserSessionList[currentKey];
            return null;
        }
    }

    public static UserVO? CurrentUser
    {
        get
        {
            if (CurrentKey is Guid currentKey && currentKey != Guid.Empty)
                return UserSessionList[currentKey].CurrentUser;
            return null;
        }
        private set
        {
            UserSessionList[CurrentKey].CurrentUser = value;
        }
    }

    public static void CreateNewOnlineUserSession()
    {
        Guid userSessionKey = CurrentKey;
        if (!UserSessionList.ContainsKey(userSessionKey))
            UserSessionList.Add(userSessionKey , new UserSession());
    }

    public static void SetOnlineUserSessionKey(Guid userSessionKey)
    {
        _httpContext.HttpContext.Items[NameKeys.UserSessionKey] = userSessionKey;
    }

    public static void LogIn(UserVO user , SignInDTO signIn)
    {
        SetOnlineUserSessionKey(user.Id);
        CreateNewOnlineUserSession();
        CurrentUser = user;
    }

    // Remove UserSession from _httpContext.HttpContext.Items by key
    public static void LogOut()
    {
        if (_httpContext.HttpContext != null &&
        _httpContext.HttpContext.Items != null &&
        IskeyExist)
        {
            UserSessionList.Remove(CurrentKey);
            _httpContext.HttpContext.Items.Remove(NameKeys.UserSessionKey);
        }
    }

    //public static void ServiceLogIn()
    //{
    //    _httpContext.HttpContext = new DefaultHttpContext();
    //    SetOnlineUserSessionKey(Guid.Empty);
    //    CreateNewOnlineUserSession();
    //}
}
public class UserSession
{
    public UserVO CurrentUser { get; set; }
    public bool RightToLeft { get; set; }
    public bool IsAutoTranslater { get; set; }
    public bool IsAutoPost { get; set; }
    public int Fraction { get; set; }
}

