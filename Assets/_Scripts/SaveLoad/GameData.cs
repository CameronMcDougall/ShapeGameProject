using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class GameData {

    public string levelName;
    public string checkPointName;
    public string timestamp;

    public GameData(string lvlName, string CheckPName) {
        levelName = lvlName;
        checkPointName = CheckPName;
        timestamp = System.DateTime.Now.ToString();
    }
    public bool Equals(GameData cmp) {
        if (cmp.levelName == this.levelName && cmp.checkPointName == this.checkPointName) {
            return true;
        }
        return false;
    }
}
[System.Serializable]
public class QueueGamedata {

    public Queue<GameData> savesQueue;

    public QueueGamedata(Queue<GameData> queue)
    {
        savesQueue = queue;
    }

}
public class StaticCheckpoint {
    public static string spawn_point { get; set; }
}
