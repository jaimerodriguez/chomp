#if  USEXBOXLIVE
using UnityEngine;
using System.Collections;
using Microsoft.Plugins.XBL;

using System;
using Microsoft.UnityPlugins;
using System.Collections.Generic;

public class XboxLiveUser  {
 
    SignInResult xboxLiveResult;
  

    bool hasFailedLogin = false;
    static bool useXbox = false; 

    public XboxLiveUser()
    {

    }
 
    public XboxLiveUser(  SignInResult xblResult )
    {
        xboxLiveResult = xblResult;
        hasFailedLogin = (xblResult.SignInStatus == SignInStatus.UserCancel); 
    }
 

    public bool IsLoggedIn
    {
        get
        {
            return  (xboxLiveResult!= null &&  
                    xboxLiveResult.SignInStatus == SignInStatus.Success); 
        }
    }

    public static bool UseIdentity
    {
        get
        {  
            return useXbox && !PlatformHelper.IsWindowsMobile;
        }
    } 

    public bool HasFaileLogin
    {
        get { return hasFailedLogin;  }
        set { hasFailedLogin = value;  }
    }

    public bool NeedsLogin
    {
        get
        {
            return UseIdentity && !hasFailedLogin && !IsLoggedIn; 
        }
    }

    public string Gamertag { get  
        {  
            if (IsLoggedIn ) 
                return xboxLiveResult.User.Gamertag; 
        }
    } 


    public IChompUser  User
    {
        get
        {
            if (IsLoggedIn)
                return xboxLiveResult.User;
            else
                return null; 
        }
    }

 

    public Profile Profile
    {
        get
        {
            if (IsLoggedIn)
                return xboxLiveResult.Profile;
            return null; 
        }
    }

    public void GetAchievements( Action<CallbackResponse<IList<AchievementData>>> action) 
    {
        if (this.IsLoggedIn ) 
            XboxLiveBridge.Instance.GetAchievements(xboxLiveResult.User.XboxUserId, action);

    }


    public void GetLeaderboard( string leaderboardName, Action<CallbackResponse<LeaderboardData>> action)
    {
        if (this.IsLoggedIn)
            XboxLiveBridge.Instance.GetLeaderboard(leaderboardName, action);  

    }

}

#endif 
