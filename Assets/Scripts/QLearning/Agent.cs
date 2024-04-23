using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{

    List<Hex> placeAbleArea = new();
    Shop shop;
    GameManager gameManager;
    int rewardValue = -1;

    private void Start()
    {
        shop = GameObject.Find("Shop").GetComponent<Shop>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    public int MoveSoldier(Soldier soldier, Hex targetHex)
    {
        if (soldier.onHex.travelContinentByStepForSoldier(4, soldier.owner, soldier.soldierLevel).Contains(targetHex) && !soldier.hasMoved)
        {
            soldier.walkToHex(targetHex);
            Debug.Log("MOVED");
            if (targetHex.HexObjectType == ObjectType.SoldierLevel1 ||
                targetHex.HexObjectType == ObjectType.SoldierLevel2 ||
                targetHex.HexObjectType == ObjectType.SoldierLevel3 ||
                targetHex.HexObjectType == ObjectType.SoldierLevel4)
            {
                rewardValue = 20;
            }
            if (targetHex.HexObjectType == ObjectType.TreeWeak || targetHex.HexObjectType == ObjectType.Tree)
            {
                rewardValue = 5;
            }
            if (targetHex.HexObjectType == ObjectType.BuildingDefenceLevel1 || targetHex.HexObjectType == ObjectType.BuildingDefenceLevel2)
            {
                rewardValue = 10;
            }
            if (targetHex.Owner != soldier.owner)
            {
                rewardValue = 7;
            }
            if (targetHex.HexObjectType == ObjectType.TownHall)
            {
                rewardValue = 50;
            }
            if (targetHex.HexObjectType == ObjectType.None && targetHex.Owner == soldier.owner)
            {
                rewardValue = -3;
            }
        }
        else
        {
            rewardValue = -50;
        }
        return rewardValue;
    }
    public int BuySoldier(Player player, Hex targetHex, ObjectType objectType, int cost)
    {
        placeAbleArea.Clear();
        Hex startHex = player.ownedHexes[0];
        placeAbleArea = startHex.travelContinentByStepForSoldier(50, player, objectType);
        if (placeAbleArea.Contains(targetHex))
        {
            if (targetHex.Owner != player)
            {
                rewardValue = 7;
            }
            if (targetHex.HexObjectType == ObjectType.TreeWeak || targetHex.HexObjectType == ObjectType.Tree)
            {
                rewardValue = 5;
            }
            if (player.economyManager.CalculateIncome() < 0)
            {
                rewardValue = -20;
            }
            shop.BuySomethingOnTargetHex(targetHex, player, cost, objectType);
        }
        else
        {
            rewardValue = -10;
        }
        return rewardValue;
    }
    public int BuyTower1(Player player, Hex targetHex)
    {
        placeAbleArea.Clear();
        foreach (Hex hex in player.ownedHexes)
        {
            foreach (Hex hex1 in hex.neighbors)
            {
                if (player.ownedHexes.Contains(hex1) && hex1.HexObjectType == ObjectType.None)
                {
                    placeAbleArea.Add(hex1);
                }
            }
        }
        if (placeAbleArea.Contains(targetHex))
        {
            shop.BuySomethingOnTargetHex(targetHex, player, 15, ObjectType.BuildingDefenceLevel1);
            if (targetHex.neighbors.Any(hex => hex.HexObjectType == ObjectType.BuildingDefenceLevel1) ||
            targetHex.neighbors.Any(hex => hex.HexObjectType == ObjectType.BuildingDefenceLevel2))
            {
                rewardValue = -5;
            }
            if (player.economyManager.CalculateIncome() < 0)
            {
                rewardValue = -20;
            }
        }
        else
        {
            rewardValue = -10;
        }
        return rewardValue;
    }
    public int BuyTower2(Player player, Hex targetHex)
    {
        placeAbleArea.Clear();
        foreach (Hex hex in player.ownedHexes)
        {
            foreach (Hex hex1 in hex.neighbors)
            {
                if (player.ownedHexes.Contains(hex1) && hex1.HexObjectType == ObjectType.None)
                {
                    placeAbleArea.Add(hex1);
                }
            }
        }
        if (placeAbleArea.Contains(targetHex))
        {
            shop.BuySomethingOnTargetHex(targetHex, player, 35, ObjectType.BuildingDefenceLevel2);
            if (targetHex.neighbors.Any(hex => hex.HexObjectType == ObjectType.BuildingDefenceLevel1) ||
            targetHex.neighbors.Any(hex => hex.HexObjectType == ObjectType.BuildingDefenceLevel2))
            {
                rewardValue = -5;
            }
            if (player.economyManager.CalculateIncome() < 0)
            {
                rewardValue = -20;
            }
        }
        else
        {
            rewardValue = -10;
        }
        return rewardValue;
    }
    public int BuyFarm(Player player, Hex targetHex)
    {
        rewardValue = -1;
        placeAbleArea.Clear();
        foreach (Hex hex in player.ownedHexes)
        {
            if (hex.HexObjectType == ObjectType.BuildingFarm || hex.HexObjectType == ObjectType.TownHall)
            {
                foreach (Hex hex1 in hex.neighbors)
                {
                    if (player.ownedHexes.Contains(hex1) && hex1.HexObjectType == ObjectType.None)
                    {
                        placeAbleArea.Add(hex1);
                    }
                }
            }
        }
        if (placeAbleArea.Contains(targetHex))
        {
            int cost = 12;
            int farmCount = GridSystem.CountChoosenObjectOnHexes(player.ownedHexes, ObjectType.BuildingFarm);
            cost = cost + farmCount * 5;
            rewardValue = 1;
            shop.BuySomethingOnTargetHex(targetHex, player, cost, ObjectType.BuildingFarm);
        }
        else
        {
            rewardValue = -10;
        }
        return rewardValue;
    }

    public int EndTurn(Player player)
    {
        bool allSoldiersMoved = true;
        if (player.soldiers.Count > 0)
        {
            foreach (Soldier soldier in player.soldiers)
            {
                if (soldier.hasMoved == false)
                {
                    allSoldiersMoved = false;
                }
            }
        }
        if (allSoldiersMoved == true)
        {
            rewardValue = 10;
        }
        else
        {
            rewardValue = -5;
        }
        gameManager.endTurn();
        return rewardValue;
    }



}
