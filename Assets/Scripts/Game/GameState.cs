using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
#if USEXBOXLIVE 
using Microsoft.Plugins.XBL;
#endif 

namespace MonsterGame
{
    public class GameState
    {

        protected GameState() {
            Lives = Constants.DefaultLives;
#if DEBUG 
            IsInvincible = false;
#endif 
        }

        static GameState instance;

        public static GameState Instance { get
            {
                if (instance == null)
                    instance = new GameState();
                return instance;
            }
        }

        public int Score { get; private set; }
        public int Lives { get; private set; }
        public IChompUser User 
        { 
            get
            {
                if (chompUser == null)
                {  
                    
                   chompUser =  UserFactory.Create(); 
                     

                }
                return chompUser; 
            } 
            internal set  {
                chompUser = value; 
            } 
        } 


#if DEBUG
        internal Direction PlayerDirection { get; set; }
        internal Direction MonsterDirection { get; set; }

        List<string> pills = new List<string>();
        internal void AddPill(string tag)
        {
            //    pills.Add(tag); 
        }

        internal void RemovePill(string tag)
        {
            //     pills.Remove(tag); 
        }

        public bool IsInvincible { get; set; }
#endif 

        private int pillsEaten;
        private int prizesEaten;
        private int targetPills;
        private int targetPrizes;
        private int currentLevelIndex = -1;
        private Vector2 _playerPosition;
        private IChompUser chompUser; 

        public void EatPill()
        {
            lock (this)
            {
                Score += Constants.PillValue;
                pillsEaten++;
                CheckWin();
            }

        }



#if DEBUG
        public int PillsLeft { get { return targetPills - pillsEaten; } }
        public int PrizesLeft { get { return targetPrizes - prizesEaten; } }
#endif 


        void CheckWin()
        {
            if (pillsEaten == targetPills && prizesEaten == targetPrizes)
            {
                Win();
            }
        }
        public void InitLevel(int level, int targetPills, int targetPrizes, MazeConfig config)
        {
            this.targetPills = targetPills;
            this.targetPrizes = targetPrizes;
            this.currentLevelIndex = level;
            this._status = GameStatus.Loading;
            Lives = Constants.DefaultLives;
            this.pillsEaten = 0;
            this.prizesEaten = 0;
            MazeConfiguration = config;  
        }

        public MazeConfig MazeConfiguration {  get ; private set ; } 

        public void EatFeature (   )
        {
            lock (this)
            {  
                SoundManager.Instance.PlayEat(); 
                Score += Constants.FeatureValue;
                prizesEaten++;
                CheckWin();
            }
        }

        public void ResetLevel ()
        {

        }

        public bool HasLives
        {
            get
            {
                return Lives >= 0; 
            }
        }

        public void Die ()
        {
            if (Lives == 0)            
            {
                SetState(GameStatus.GameOver);     
            }
            else if (Lives > 0)
            {
                SetState(GameStatus.Restarting); 

            }
            else
                return; 
            Lives--;
            Debug.Log("lives decreased"); 
        }

        public void Win ()
        {
            SetState(GameStatus.Win); 
        }

        public bool IsPaused
        {  get
            {
                return _status == GameStatus.Paused ; 
            }
        }

        public bool IsRunning
        {
            get
            {
                return _status == GameStatus.Playing ; 
            } 
        }

        public bool IsWin
        {
            get
            {
                return _status == GameStatus.Win;
            }
        }
        public bool IsRestarting
        {
            get
            {
                return _status == GameStatus.Restarting;
            }
        }

        public bool IsLoading
        {
            get
            {
                return _status == GameStatus.Loading;
            }
        }

        public bool IsReady
        {
            get
            {
                return _status == GameStatus.GetReady; 
            }
        }

        public bool IsGameOver
        {
            get
            {
                return _status == GameStatus.GameOver;
            }
        }

        public bool IsGameOverCompleted
        {
            get
            {
                return _status == GameStatus.GameOverCompleted;
            }
        }

        public bool IsLifeLostCompleted
        {
            get
            {
                return _status == GameStatus.LifeLostCompleted  ;
            }
        }
        public bool IsLifeLost
        {
            get
            {
                return _status == GameStatus.LifeLost;
            }
        }

        public bool IsWinCompleted
        {
            get
            {
                return _status == GameStatus.WinCompleted ;
            }
        }

        public void UnPause ()
        {
            SetState(oldStatus); 
        }

        public void SetState ( GameStatus status )
        {
#if DEBUG 
            if ( status != _status )                
#endif

            {
                Debug.Log("new status == " + status.ToString());
                oldStatus = _status;
                _status = status;
                if (oldStatus == GameStatus.Win && status == GameStatus.PlayingAdvertisement)
                {
                    OnStatusChanged(status, oldStatus);
                }
                else if (status == GameStatus.WinCompleted && oldStatus == GameStatus.PlayingAdvertisement)
                {
                    Debug.Assert(false, "this should not happen"); 
                    //ignore... we are playing ad.. fire WinCompleted when ad is completed      
                }                
                else
                    OnStatusChanged(status, oldStatus);
            }   
                      
        }
        GameStatus oldStatus; 

        void OnStatusChanged ( GameStatus newState , GameStatus oldState )
        {
            EventHandler<GameStatusChangedEventArgs> eh = this.StatusChanged; 
            if ( eh != null )
            {
                eh(this, new GameStatusChangedEventArgs(newState, oldState)); 
            }
        }


        public GameStatus GetState()
        {
            return _status;   
        }

        public Vector2 PlayerPosition
        {
            get
            {
                return _playerPosition; 
            }
        } 
        public  void SetPlayerCoords ( Vector2 position )
        {
            _playerPosition = position; 
        }
        

        Point GetTilePosition ( Vector2 coords )
        {
            return new Point(0, 0); 
        }


        GameStatus _status; 

        public int GetNextLevel()
        {
            return ++currentLevelIndex; 
        }

        public int CurrentLevel
        {
            get { return currentLevelIndex;  }
        }

        public ILevel GetLevel ( int index )
        {
            return Levels.AllLevels[ index  % Levels.AllLevels.Length]; 
        }

        public enum GameStatus
        {
            Unknown, 
            Loading,
            GetReady, 
            GetReadyCompleted, 
            Playing, 
            Paused,    
            LifeLost,   
            LifeLostCompleted,       
            GameOver,
            GameOverCompleted,  
            Win, 
            WinCompleted , 
            Restarting,
            PlayingAdvertisement,
            AdvertisementCompleted
        }

        static class Constants
        {
            public const int PillValue = 10;
            public const int DefaultLives = 2;
            public const int FeatureValue = 100;  
        }

        public float MusicVolume { get; set;  }
        public float FXVolume { get; set;   }
        
        public bool PlayMusic { get; set;  }
        public bool PlayFx { get; set;  }


        public event EventHandler<GameStatusChangedEventArgs> StatusChanged; 
        
    }

    public class GameStatusChangedEventArgs : EventArgs
    { 
        public GameStatusChangedEventArgs(GameState.GameStatus newState, GameState.GameStatus oldState)
        {
            this.NewState = newState;
            this.OldState = oldState;
        }

        public GameState.GameStatus NewState { get; private set;  }
        public GameState.GameStatus OldState { get; private set;  }

    }


    
}
