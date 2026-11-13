using Data.Common;
using Data.Managers;
using Data.VO;
using System.Globalization;
using System.Text.Json;

namespace BrilliantBooking.Services;

public class SessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ISession Session
    {
        get
        {
            return _httpContextAccessor?.HttpContext?.Session;
        }
    }
    public bool HasValidSessionKeys
    {
        get
        {
            if (_httpContextAccessor?.HttpContext?.Session is null)
                return false;
            return UserSessionKey is Guid userSessionKey && userSessionKey != Guid.Empty;
        }
    }
    public bool IskeyExist
    {
        get
        {
            return Session?.Keys.Contains(NameKeys.UserSessionKey) ?? false;
        }
    }

    public Guid? UserSessionKey { get { return IskeyExist ? GetObject<Guid?>(NameKeys.UserSessionKey) : null; } }

    public SessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #region Session Methods

    public async Task SetObject<T>(string key , T value)
    {

        await Session.LoadAsync();
        // Only use one session object - don't set both
        // Serialize the object to a JSON string and store it in the session
        //Session.SetString(key , JsonSerializer.Serialize(value));
        Session.SetString(key , JsonSerializer.Serialize(value));
        await Session.CommitAsync();
    }
    public T GetObject<T>(string key)
    {
        if (Session is null)
            return default;
        // Retrieve the JSON string from the session
        var value = Session.GetString(key);

        // If the key doesn't exist in the session, return the default value for the type
        // Otherwise, deserialize the JSON string back into the object and return it
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
    public void Remove(string key)
    {
        Session.Remove(NameKeys.UserSessionKey);
    }

    #endregion Session Methods

    #region User Session Methods

    public void SendOnlineUserKeyToBackEnd()
    {
        if (UserSessionKey is Guid userSessionKey)
        {
            UserSessionManager.SetOnlineUserSessionKey(userSessionKey);
            // Example: Get current UI Culture
            UserSessionManager.CurrentUserSession.RightToLeft = CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
        }
        else
        {

        }
    }

    public async Task SetOnlineUserSessionAsync(UserVO User)
    {
        await SetObject(NameKeys.UserSessionKey , UserSessionManager.CurrentKey);
    }

    //public async Task LoginAsPublicUser()
    //{
    //    if (IsPublicUser)
    //        return;
    //    Guid? userId = GetObject<Guid?>(NameKeys.UserSessionKey);
    //    OnlineUserVO onlineUser = await new OnlineUserFDM().LogInPublic(userId);

    //    //await SessionService.SetObject(NameKeys.UserSessionKey , UserSessionManager.CurrentKey);
    //    await SetOnlineUserSessionAsync(onlineUser);
    //}
    public async Task LogOutUserSessionKeys()
    {
        //await new UserTrackerFDM().LogOut();
        //await LoginAsPublicUser();

    }
    #endregion
}
