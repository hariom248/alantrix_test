
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardState
{
    public int cardId;
    public CardVisualState cardVisualState;
}

[System.Serializable]
public class GameSaveData
{
    public int gridWidth;
    public int gridHeight;
    public int score;
    public int moves;
    public int matchedPairs;
    public int comboMultiplier;
    public List<CardState> cardStates;
}

public class GameDataManager
{
    private const string SaveKey = "SaveData";

    public void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public bool TryLoad(out GameSaveData data)
    {
        string json = PlayerPrefs.GetString(SaveKey);
        if (string.IsNullOrEmpty(json))
        {
            data = default;
            return false;
        }

        data = JsonUtility.FromJson<GameSaveData>(json);
        return true;
    }
    
    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}