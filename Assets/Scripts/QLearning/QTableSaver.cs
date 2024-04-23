using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;
public class QTableSaver
{


}
[System.Serializable]
public class QTableData
{
    public List<GameStateData> States;
}

[System.Serializable]
public class GameStateData
{
    public GameState State;
    public Dictionary<string, Dictionary<string, double>> ActionValues;

    public GameStateData(GameState state, Dictionary<GameAction, double> actionValues)
    {
        State = state;
        ActionValues = new Dictionary<string, Dictionary<string, double>>();

        foreach (var kvp in actionValues)
        {
            string actionJson = kvp.Key.ToJson();
            ActionValues[actionJson] = new Dictionary<string, double> { { "Value", kvp.Value } };
        }
    }
}

public class QTableConverter : MonoBehaviour
{
    // Örnek bir CurrentQTable

    /*void Convert(Dictionary<GameState, Dictionary<GameAction, double>> CurrentQTable)
    {
        
        // QTableData'ya çevir
        QTableData qTableData = ConvertToQTableData(CurrentQTable);

        // QTableData'yı JSON'a dönüştür
        string json = JsonUtility.ToJson(qTableData);
        Debug.Log(json);

        // JSON'dan QTableData'ya geri çevir
        QTableData parsedData = JsonUtility.FromJson<QTableData>(json);

        // QTableData'yı tekrar CurrentQTable'a çevir
        Dictionary<GameState, Dictionary<GameAction, double>> newQTable = ConvertToCurrentQTable(parsedData);
    }*/

    // CurrentQTable'ı QTableData'ya çeviren metot
    public static QTableData ConvertToQTableData(Dictionary<GameState, Dictionary<GameAction, double>> qTable)
    {
        QTableData qTableData = new QTableData
        {
            States = new List<GameStateData>()
        };

        foreach (var stateEntry in qTable)
        {
            GameStateData stateData = new GameStateData(stateEntry.Key, stateEntry.Value);
            qTableData.States.Add(stateData);
        }

        return qTableData;
    }

    // QTableData'yı CurrentQTable'a çeviren metot
    public static Dictionary<GameState, Dictionary<GameAction, double>> ConvertToCurrentQTable(QTableData qTableData)
    {
        Dictionary<GameState, Dictionary<GameAction, double>> qTable = new Dictionary<GameState, Dictionary<GameAction, double>>();

        foreach (var stateData in qTableData.States)
        {
            Dictionary<GameAction, double> actionValues = new Dictionary<GameAction, double>();

            foreach (var actionEntry in stateData.ActionValues)
            {
                GameAction action = JsonUtility.FromJson<GameAction>(actionEntry.Key);
                double value = actionEntry.Value["Value"];
                actionValues[action] = value;
            }

            qTable[stateData.State] = actionValues;
        }

        return qTable;
    }
}

