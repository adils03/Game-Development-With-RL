using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class QLearning : MonoBehaviour
{
    [SerializeField] Agent agent;
    [SerializeField] float waitTime;
    [SerializeField] bool canExplore = true;
    [SerializeField] int countLimit;
    [SerializeField] private float explorationRate = 0.1f;
    Dictionary<GameState, Dictionary<GameAction, double>> CurrentQTable;
    List<GameAction> currentActions = new List<GameAction>();
    HashSet<Hex> placableArea = new HashSet<Hex>();
    HashSet<Hex> soldierPositions = new HashSet<Hex>();

    GameManager gameManager;
    GridSystem gridSystem;
    int rewardAmount;
    private void Start()
    {
        gridSystem = GameObject.Find("GridSystem").GetComponent<GridSystem>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.RestartGame();
        StartCoroutine(Learn());
    }
    IEnumerator Learn()
    {
        //CurrentQTable = QTableSaver.LoadQTable(); düzeltilecek
        List<GameAction> gameActions = QTable.AssignGameActions(gridSystem);
        CurrentQTable = new Dictionary<GameState, Dictionary<GameAction, double>>();
        // Pekiştirmeli öğrenme döngüsü
        for (int episode = 0; episode < 1000000; episode++)
        {
            Player currentPlayer = gameManager.GetTurnPlayer();
            int count = 0;
            GameState State = new GameState(currentPlayer, QTable.DetectMapStatus(gridSystem));
            CurrentQTable = QTable.UpdateQTable(currentPlayer, CurrentQTable, gridSystem, gameActions);
            LimitActions(currentPlayer, gameActions);
            while (true)
            {
                GameAction action;
                currentPlayer = gameManager.GetTurnPlayer();
                Debug.Log("GameState= " + State.TotalGold + " + " + State.TotalIncome);
                if (UnityEngine.Random.Range(0.0f, 1.0f) < explorationRate || canExplore)  // %10 olasılıkla rastgele bir eylem seç
                {
                    action = currentActions[UnityEngine.Random.Range(0, currentActions.Count)];
                }
                else  // %90 olasılıkla en yüksek Q-değerine sahip eylemi seç
                {
                    action = MaxQAction(GetSpecificActions(State, currentActions));
                }
                GameState nextState = PerformAction(currentPlayer, action);
                double reward = rewardAmount;
                // Q-değerini güncelle
                double oldQ = CurrentQTable[State][action];
                double maxNextQ = MaxQValue(CurrentQTable[State]);
                double newQ = oldQ + 0.1 * (reward + 0.9 * maxNextQ - oldQ);
                CurrentQTable[State][action] = newQ;
                if (State != nextState)
                {
                    LimitActions(currentPlayer, gameActions);
                    CurrentQTable = QTable.UpdateQTable(currentPlayer, CurrentQTable, gridSystem, gameActions);
                }
                // Yeni duruma geç
                State = nextState;
                if (GridSystem.CountChoosenObjectOnHexes(gridSystem.hexes, ObjectType.TownHall) == 1 || count == countLimit)
                {
                    gameManager.RestartGame();
                    countLimit += 10;
                    break;
                }
                count++;
                //SaveQTable();
                //QTableSaver.SaveQTable(CurrentQTable); düzeltilecek
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    GameState PerformAction(Player player, GameAction action)
    {

        if (action.actionType == GameAction.ActionType.MoveSoldier)
        {
            if (gridSystem.FindHex(action.StartCoordinates.Item1, action.StartCoordinates.Item2).HexObjectType == ObjectType.SoldierLevel1 ||
                gridSystem.FindHex(action.StartCoordinates.Item1, action.StartCoordinates.Item2).HexObjectType == ObjectType.SoldierLevel2 ||
                gridSystem.FindHex(action.StartCoordinates.Item1, action.StartCoordinates.Item2).HexObjectType == ObjectType.SoldierLevel3 ||
                gridSystem.FindHex(action.StartCoordinates.Item1, action.StartCoordinates.Item2).HexObjectType == ObjectType.SoldierLevel4)
            {
                rewardAmount = agent.MoveSoldier(gridSystem.FindHex(action.StartCoordinates.Item1, action.StartCoordinates.Item2).ObjectOnHex.GetComponent<Soldier>(), gridSystem.FindHex(action.TargetCoordinates.Item1, action.TargetCoordinates.Item2));
            }
        }
        else { rewardAmount = -50; }

        if (action.actionType == GameAction.ActionType.PlaceSoldier1)
        {
            rewardAmount = agent.BuySoldier(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2), ObjectType.SoldierLevel1, 10);
        }

        if (action.actionType == GameAction.ActionType.PlaceSoldier2)
        {
            rewardAmount = agent.BuySoldier(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2), ObjectType.SoldierLevel2, 20);
        }

        if (action.actionType == GameAction.ActionType.PlaceSoldier3)
        {
            rewardAmount = agent.BuySoldier(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2), ObjectType.SoldierLevel3, 30);
        }

        if (action.actionType == GameAction.ActionType.PlaceSoldier4)
        {
            rewardAmount = agent.BuySoldier(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2), ObjectType.SoldierLevel4, 40);
        }

        if (action.actionType == GameAction.ActionType.PlaceTower1)
        {
            rewardAmount = agent.BuyTower1(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2));
        }

        if (action.actionType == GameAction.ActionType.PlaceTower2)
        {
            rewardAmount = agent.BuyTower2(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2));
        }

        if (action.actionType == GameAction.ActionType.PlaceFarm)
        {
            rewardAmount = agent.BuyFarm(player, gridSystem.FindHex(action.Coordinates.Item1, action.Coordinates.Item2));
        }

        if (action.actionType == GameAction.ActionType.EndTurn)
        {
            rewardAmount = agent.EndTurn(player);
        }

        return new GameState(player, QTable.DetectMapStatus(gridSystem));
    }


    GameAction MaxQAction(Dictionary<GameAction, double> actionValues)
    {
        // Bu fonksiyon, belirli bir durum için en yüksek Q-değerine sahip eylemi döndürür.
        GameAction maxQAction = null;
        double maxQValue = double.NegativeInfinity;
        foreach (KeyValuePair<GameAction, double> entry in actionValues)
        {
            if (entry.Value > maxQValue)
            {
                maxQValue = entry.Value;
                maxQAction = entry.Key;
            }
        }
        return maxQAction;
    }

    double MaxQValue(Dictionary<GameAction, double> actionValues)
    {
        // Bu fonksiyon, belirli bir durum için en yüksek Q-değerini döndürür.
        double maxQValue = double.NegativeInfinity;
        foreach (KeyValuePair<GameAction, double> entry in actionValues)
        {
            if (entry.Value > maxQValue)
            {
                maxQValue = entry.Value;
            }
        }
        return maxQValue;
    }

    void LimitActions(Player player, List<GameAction> _gameActions)
    {
        currentActions.Clear();
        soldierPositions.Clear();
        placableArea.Clear();
        placableArea = player.ownedHexes[0].travelContinentByStepForSoldier(50, player, ObjectType.SoldierLevel4).ToHashSet();

        for (int i = 0; i < _gameActions.Count; i++)
        {
            if (_gameActions[i].actionType != GameAction.ActionType.EndTurn && _gameActions[i].actionType != GameAction.ActionType.MoveSoldier && player.PlayerTotalGold > 4)
            {
                if (placableArea.Contains(gridSystem.FindHex(_gameActions[i].Coordinates.Item1, _gameActions[i].Coordinates.Item2)))
                {
                    currentActions.Add(_gameActions[i]);
                }
            }
            else if (_gameActions[i].actionType == GameAction.ActionType.MoveSoldier && player.soldiers.Count > 0)
            {
                foreach (Hex hex in player.ownedHexes)
                {
                    if (hex.HexObjectType == ObjectType.SoldierLevel1 ||
                    hex.HexObjectType == ObjectType.SoldierLevel2 ||
                    hex.HexObjectType == ObjectType.SoldierLevel3 ||
                    hex.HexObjectType == ObjectType.SoldierLevel4)
                    {
                        soldierPositions.Add(hex);
                    }
                }
                if (soldierPositions.Contains(gridSystem.FindHex(_gameActions[i].StartCoordinates.Item1, _gameActions[i].StartCoordinates.Item2)) &&
                placableArea.Contains(gridSystem.FindHex(_gameActions[i].TargetCoordinates.Item1, _gameActions[i].TargetCoordinates.Item2)))
                {
                    currentActions.Add(_gameActions[i]);
                }
            }
            else if (_gameActions[i].actionType == GameAction.ActionType.EndTurn)
            {
                currentActions.Add(_gameActions.Last());
            }
        }
    }

    public Dictionary<GameAction, double> GetSpecificActions(GameState state, List<GameAction> specificActions)
    {
        Dictionary<GameAction, double> actionValues = new Dictionary<GameAction, double>();

        if (CurrentQTable.ContainsKey(state))
        {
            foreach (GameAction action in specificActions)
            {
                if (CurrentQTable[state].ContainsKey(action))
                {
                    actionValues[action] = CurrentQTable[state][action];
                }
            }
        }

        return actionValues;
    }

    private void SaveQTable()
    {
        QTableData qTableData = QTableConverter.ConvertToQTableData(CurrentQTable);


        string json = JsonUtility.ToJson(qTableData);
        File.WriteAllText("QTable.json", json);
    }

}

