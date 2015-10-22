using System;
using UnityEngine;
using System.Collections;

using Microsoft.UnityPlugins; 
using System.Collections.Generic;

public interface IChompUser
{
    bool IsLoggedIn { get; }
    bool HasFaileLogin { get; set; }
    bool NeedsLogin { get; }

    string Gamertag { get;  }

//void GetAchievements(Action<CallbackResponse<IList<AchievementData>>> action);

//    void GetLeaderboard(string leaderboardName, Action<CallbackResponse<LeaderboardData>> action); 
}

public class UserFactory
{
    
    public static bool UseIdentity
    {
        get
        {
#if USEXBOXLIVE
            return true ; 
#endif
            return false; 

        }
    }

    public static IChompUser Create()
    {
        if (UseIdentity)
        {
#if USEXBOXLIVE
            return new XboxLiveUser ( ) ; 
#endif
            
        }
        return new DummyUser();
    }

}


public class DummyUser : IChompUser
{
    public bool IsLoggedIn { get { return false;  }  }
    public bool HasFaileLogin { get; set;  }
    public bool NeedsLogin { get { return false;  } }
    public string Gamertag { get { return "User";  } }


    /* 
    public void GetAchievements(Action<CallbackResponse<IList<AchievementData>>> action)
    {
       throw new NotImplementedException();

    }


    public void GetLeaderboard(string leaderboardName, Action<CallbackResponse<LeaderboardData>> action)
    {
       throw new NotImplementedException();
    }*/ 
}