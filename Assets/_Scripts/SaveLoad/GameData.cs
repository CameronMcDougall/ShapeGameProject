using System.Collections.Generic;

[System.Serializable]
public class GameData {

    public string levelName;
    public string checkPointName;

    public GameData(string lvlName, string CheckPName) {
        levelName = lvlName;
        checkPointName = CheckPName;
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
