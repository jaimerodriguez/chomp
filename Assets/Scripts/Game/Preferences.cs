using UnityEngine;
using System.Collections;
using System; 

namespace MonsterGame
{
    public class Preferences
    {
        internal const string FXVolumeKey = "FXVolume";
        internal const string MusicVolumeKey = "MusicVolume";
        internal const string XboxLiveKey = "XboxLive";
        internal const string LiveTilesKey = "LiveTiles";
        internal const string PlayerSpeedKey = "PlayerSpeed";

        private static Preferences _current;
        private Preferences()
        {
            Load();
        }

        public static Preferences Current
        {
            get
            {
                if (_current == null)
                    _current = new Preferences();
                return _current;
            }
        }

        //float playerSpeed , fxVolume, musicVolume; 
        //bool useXboxLive, useLiveTiles; 

        public float PlayerSpeed { get; set; }
        public float FXVolume { get; set; }
        public float MusicVolume { get; set; }

        public bool UseXboxLive { get; set; }
        public bool UseLiveTiles { get; set; }

        public void Load()
        {
            PlayerSpeed = LoadValue<float>(PlayerSpeedKey, 0.4f);
            MusicVolume = LoadValue<float>(MusicVolumeKey, 0.2f);
            FXVolume = LoadValue<float>(FXVolumeKey, 0.3f);
            UseXboxLive = LoadValue<bool>(XboxLiveKey, false);
            UseLiveTiles = LoadValue<bool>(LiveTilesKey, true);
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(PlayerSpeedKey, PlayerSpeed);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.SetFloat(FXVolumeKey, FXVolume);
            PlayerPrefs.SetInt(LiveTilesKey, (UseLiveTiles ? 1 : 0));
            PlayerPrefs.SetInt(XboxLiveKey, (UseXboxLive ? 1 : 0));
            PlayerPrefs.Save();
        }

        public T LoadValue<T>(string key, T defaultValue)
        {
            if (PlayerPrefs.HasKey(key))
            {
                if (typeof(T) == typeof(float))
                {
                    T val = (T)Convert.ChangeType(PlayerPrefs.GetFloat(key), typeof(T));
                }
                else if (typeof(T) == typeof(bool))
                {
                    int val = PlayerPrefs.GetInt(key);
                    return (T)Convert.ChangeType(val != 0, typeof(T));
                }
                else
                {
                    Debug.Assert(false, "type not suppported");
                    return defaultValue;
                }
            }

            return defaultValue;
        }
    }
} 