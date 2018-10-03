[System.Serializable]
public class GameData {

    public string levelName;
    public string checkPointName;

    public GameData(string lvlName, string CheckPName) {
        levelName = lvlName;
        checkPointName = CheckPName;
    }
}
