using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Levels
{
    [Serializable]
    public class GameSessionData
    {
        public int savedAmount;
        public float timeSpent;
        public int healthSpent;
        public float fuelSpent;
    }

    [Serializable]
    public class GameSessionTable
    {
        public List<GameSessionData> sessions = new List<GameSessionData>();
        
        private const string GameSessionTableKey = nameof(GameSessionTable);
        
        public static GameSessionTable Load()
        {
            var json = PlayerPrefs.GetString(GameSessionTableKey);

            try
            {
                return JsonUtility.FromJson<GameSessionTable>(json);
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public static void Save(GameSessionData session)
        {
            var table = Load();

            if (table == null) table = new GameSessionTable();
            table.sessions.RemoveAll(s => s == null);
            table.sessions.Add(session);

            var json = JsonUtility.ToJson(table);
            
            PlayerPrefs.SetString(GameSessionTableKey, json);
            
            PlayerPrefs.Save();
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteKey(GameSessionTableKey);
            
            PlayerPrefs.Save();
        }
    }
}