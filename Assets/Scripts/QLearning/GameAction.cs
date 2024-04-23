using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameAction
{
    public enum ActionType
    {
        EndTurn,
        PlaceSoldier1,
        PlaceSoldier2,
        PlaceSoldier3,
        PlaceSoldier4,
        PlaceTower1,
        PlaceTower2,
        PlaceFarm,
        MoveSoldier
    }
    public ActionType actionType { get; set; }
    public (int, int) Coordinates { get; set; }
    public (int, int) StartCoordinates { get; set; }
    public (int, int) TargetCoordinates { get; set; }

    public GameAction(ActionType _actionType)
    {
        actionType = _actionType;
    }
    public GameAction(ActionType _actionType, (int, int) coordinates)
    {
        actionType = _actionType;
        Coordinates = coordinates;
    }

    public GameAction(ActionType _actionType, (int, int) startCoordinates, (int, int) targetCoordinates)
    {
        actionType = _actionType;
        StartCoordinates = startCoordinates;
        TargetCoordinates = targetCoordinates;
    }
    public string ToJson()
    {
        string json = "{\"ActionType\":\"" + actionType.ToString() + "\"";

        if (Coordinates != default)
            json += ",\"Coordinates\":[" + Coordinates.Item1 + "," + Coordinates.Item2 + "]";
        if (StartCoordinates != default)
            json += ",\"StartCoordinates\":[" + StartCoordinates.Item1 + "," + StartCoordinates.Item2 + "]";
        if (TargetCoordinates != default)
            json += ",\"TargetCoordinates\":[" + TargetCoordinates.Item1 + "," + TargetCoordinates.Item2 + "]";

        json += "}";

        return json;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GameAction other = (GameAction)obj;

        // ActionType ve Coordinates'ın eşit olup olmadığını kontrol et
        return (actionType == other.actionType) &&
               CoordinatesEquals(Coordinates, other.Coordinates) &&
               CoordinatesEquals(StartCoordinates, other.StartCoordinates) &&
               CoordinatesEquals(TargetCoordinates, other.TargetCoordinates);
    }

    private bool CoordinatesEquals((int, int) a, (int, int) b)
    {
        // Tuple'ların içindeki elemanları karşılaştır
        return a.Item1 == b.Item1 && a.Item2 == b.Item2;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;

            // ActionType'ın hash'ini al
            hash = hash * 23 + actionType.GetHashCode();

            // Coordinates'ın hash'ini al
            hash = hash * 23 + Coordinates.GetHashCode();
            hash = hash * 23 + StartCoordinates.GetHashCode();
            hash = hash * 23 + TargetCoordinates.GetHashCode();

            return hash;
        }
    }
}
