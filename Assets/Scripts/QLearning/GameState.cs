using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[System.Serializable]
public class GameState
{
    public int TotalGold;
    public int TotalIncome;
    public List<(ObjectType, (int, int))> MapStatus;
    
    public GameState(Player _player, List<(ObjectType, (int, int))> _mapStatus)
    {
        TotalGold = _player.PlayerTotalGold;
        MapStatus = _mapStatus;
    }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        GameState otherState = (GameState)obj;

        // Özelliklerin karşılaştırılması
        return TotalGold == otherState.TotalGold &&
               TotalIncome == otherState.TotalIncome &&
               CompareLists(MapStatus, otherState.MapStatus);
    }

    private bool CompareLists<T>(List<T> list1, List<T> list2)
    {
        if (list1 == null && list2 == null)
            return true;

        if (list1 == null || list2 == null)
            return false;

        if (list1.Count != list2.Count)
            return false;

        HashSet<T> set1 = new HashSet<T>(list1);
        HashSet<T> set2 = new HashSet<T>(list2);

        return set1.SetEquals(set2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + TotalGold.GetHashCode();
            hash = hash * 23 + TotalIncome.GetHashCode();

            foreach (var item in MapStatus)
            {
                hash = hash * 23 + item.GetHashCode();
            }

            return hash;
        }
    }



}