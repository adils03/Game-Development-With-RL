using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class QTable
{
    static List<(ObjectType, (int, int))> mapStatus ;
    public static Dictionary<GameState, Dictionary<GameAction, double>> UpdateQTable(Player player, Dictionary<GameState, Dictionary<GameAction, double>> _QTable, GridSystem gridSystem, List<GameAction> gameActions)
    {
        // DetectMapStatus ile elde ettiğiniz bilgileri kullanarak bir GameState oluşturun
        mapStatus = DetectMapStatus(gridSystem);
        GameState newState = new GameState(player, mapStatus);
        List<GameAction> GameActions = gameActions;
        // Eğer state daha önce eklenmediyse ekle
        if (!_QTable.ContainsKey(newState))
        {
            _QTable[newState] = new Dictionary<GameAction, double>();
            // Her bir GameAction için başlangıç değeriyle bir alt sözlük ekleyin
            foreach (var action in GameActions)
            {
                _QTable[newState][action] = 0.0;
            }
        }
        return _QTable;
    }

    public static List<GameAction> AssignGameActions(GridSystem gridSystem)
    {
        List<GameAction> GameActions = new List<GameAction>();
        List<(int, int)> HexCords = new List<(int, int)>();
        for (int i = 0; i < gridSystem.hexes.Count; i++)
        {
            HexCords.Add((gridSystem.hexes[i].q, gridSystem.hexes[i].r));
        }
        for (int i = 0; i < HexCords.Count; i++)
        {
            if (gridSystem.FindHex(HexCords[i].Item1, HexCords[i].Item2)._hexType != Hex.hexType.water)
            {
                GameAction placeSoldier1 = new GameAction(GameAction.ActionType.PlaceSoldier1, HexCords[i]);
                GameActions.Add(placeSoldier1);
                GameAction placeSoldier2 = new GameAction(GameAction.ActionType.PlaceSoldier2, HexCords[i]);
                GameActions.Add(placeSoldier2);
                GameAction placeSoldier3 = new GameAction(GameAction.ActionType.PlaceSoldier3, HexCords[i]);
                GameActions.Add(placeSoldier3);
                GameAction placeSoldier4 = new GameAction(GameAction.ActionType.PlaceSoldier4, HexCords[i]);
                GameActions.Add(placeSoldier4);
                GameAction placeTower1 = new GameAction(GameAction.ActionType.PlaceTower1, HexCords[i]);
                GameActions.Add(placeTower1);
                GameAction placeTower2 = new GameAction(GameAction.ActionType.PlaceTower1, HexCords[i]);
                GameActions.Add(placeTower2);
                GameAction placeFarm = new GameAction(GameAction.ActionType.PlaceFarm, HexCords[i]);
                GameActions.Add(placeFarm);
            }
            for (int j = 0; j < HexCords.Count; j++)
            {
                if (GridSystem.FindDistanceBetweenHexes(gridSystem.FindHex(HexCords[i].Item1, HexCords[i].Item2), gridSystem.FindHex(HexCords[j].Item1, HexCords[j].Item2)) < 6 &&
                gridSystem.FindHex(HexCords[i].Item1, HexCords[i].Item2)._hexType != Hex.hexType.water &&
                gridSystem.FindHex(HexCords[j].Item1, HexCords[j].Item2)._hexType != Hex.hexType.water && i != j)
                {
                    GameAction moveSoldier = new GameAction(GameAction.ActionType.MoveSoldier, HexCords[i], HexCords[j]);
                    GameActions.Add(moveSoldier);
                }
            }
        }
        GameAction endTurn = new GameAction(GameAction.ActionType.EndTurn);
        GameActions.Add(endTurn);
        return GameActions;
    }

    public static List<(ObjectType, (int, int))> DetectMapStatus(GridSystem gridSystem)
    {
        List<(ObjectType, (int, int))> MapStatus = new List<(ObjectType, (int, int))>();
        for (int i = 0; i < gridSystem.hexes.Count; i++)
        {
            MapStatus.Add((gridSystem.hexes[i].HexObjectType, (gridSystem.hexes[i].q, gridSystem.hexes[i].r)));
        }
        return MapStatus;
    }



}
