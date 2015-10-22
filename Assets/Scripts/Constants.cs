using UnityEngine;
using System.Collections;

public static class Constants {

    public const string MainMenu_AchievementsButton = "achievementsButton";
    public const string MainMenu_LeaderBoardButton = "leaderBoardButton";
    public const string MainMenu_GamerTagText = "GamertagText";
    public const string MainMenu_GamerpicImage = "GamerpicImage"; 

    
    public const string Achievements_LeaderBoardName = "ScoreLeaderboard"; 

    public const string Maze_NameText = "Name"; 
    public const string Maze_ScoreText = "Score";
    public const string Maze_MonsterCountText = "MonsterCount";
    public const string Maze_LivesText = "Lives";

    public const string Store_Owned = "Owned";


    public static Color DisabledButtonColorMask = new Color(1f, 1f, 1f, .4f);
    public static Color EnabledButtonColorMask = new Color(1f, 1f, 1f, 1f);


    public const string Display_Owned = "Owned";
    public const string Display_ConsumableFulfilled = "Your purchase is complete. Enjoy!";
    public const string Display_ErrorFulfillingConsumable = "Something went wrong. We will retry to fullfill your purchase upon restart";
    public const string Display_PurchaseFailed = "Purchase failed";
    public const string Display_LevelPurchaseCompleted = "Purchase completed. Enjoy your new levels.";
    public const string Display_NextLevelAwaits = "Level {0} awaits you!"; 
    public const string Assets_ErrorIcon = "Error.png";
    public const string Display_AppName = "Chomp!"; 

    public const string RoamingSettings_NextLevel = "NextLevel";
    public const string RoamingSettings_MaxScore = "MaxScore"; 

}
