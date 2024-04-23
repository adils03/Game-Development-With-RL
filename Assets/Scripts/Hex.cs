using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum ObjectType// hex üzerindeki nesneler 
{
    None,   // Hiçbir nesne yok
    Grave,
    Tree,
    TreeWeak,
    BuildingFarm,
    SoldierLevel1,
    SoldierLevel2,
    BuildingDefenceLevel1,
    SoldierLevel3,
    BuildingDefenceLevel2,
    TownHall,
    SoldierLevel4

}
public class Hex : MonoBehaviour
{
    public Hex parent;
    public int cost;
    public int estimatedCost;
    public hexType _hexType;
    public int q, r, s;
    public List<Hex> neighbors = new List<Hex>();
    public bool hasVisited = false;
    public int Income = 1;// hex başına gelir default 1

    public Player Owner;// kimin bu hex ,null ise kimsenin
    public GameObject ObjectOnHex = null;
    public String playerName;
    public ObjectType HexObjectType { get; set; } = ObjectType.None;// hex üzerindeki nesne asker , bina , ağaç
    public String ObjectTypeName;
    public enum hexType
    {
        grass,
        water
    }
    private void Update() {
        ObjectTypeName = HexObjectType.ToString();
    }
    public void UpdateAdvantageOrDisadvantageValue()// ağaçlardan biri mevcut ise dezavantaj var 
    {
        if (this.HexObjectType == ObjectType.Tree || this.HexObjectType == ObjectType.TreeWeak)
            Income = 0;
        else if (this.HexObjectType == ObjectType.BuildingFarm)
            Income = 5;// bu da farm binası başına verilen değer   
        else
            Income = 1;
    }

    public List<Hex> travelContinentByStepForSoldier(int step, Player _owner, ObjectType soldierLevel) //Hex'in bulunduğu konumdan istenilen adım büyüklüğü kadar alanı areaForStep'e eşitler
    {
        step++;
        int stepAmount = step;
        List<Hex> areaForStep = new List<Hex>();
        Player ownedPlayer = Owner;
        Hex startHex = this;
        Queue<Hex> queue = new Queue<Hex>();

        queue.Enqueue(startHex);

        hexType __hexType = startHex._hexType;

        while (queue.Count > 0 && step > 0) //Hexin etrafını adım sayısına göre alır
        {
            int size = queue.Count;

            for (int i = 0; i < size; i++)
            {
                Hex currentHex = queue.Dequeue();

                if (!currentHex.hasVisited && currentHex._hexType == __hexType && currentHex.Owner == ownedPlayer)
                {
                    areaForStep.Add(currentHex);
                    currentHex.hasVisited = true;
                    foreach (Hex neighbor in currentHex.neighbors)
                    {
                        if (!neighbor.hasVisited && neighbor.Owner == ownedPlayer)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            step--;
        }
        foreach (Hex hex in areaForStep)
        {
            hex.hasVisited = false;
        }
        List<Hex> toRemove = new List<Hex>();

        foreach (Hex hex in areaForStep) //Farklı bir owner varsa o toprakları çıkarır
        {
            bool anyOwnerSame = false;

            for (int i = 0; i < hex.neighbors.Count; i++)
            {
                if (hex.neighbors[i].Owner == ownedPlayer)
                {
                    anyOwnerSame = true;
                    break;
                }
            }

            if (!anyOwnerSame)
            {
                toRemove.Add(hex);
            }
        }

        foreach (Hex hex in toRemove)
        {
            areaForStep.Remove(hex);
        }
        toRemove.Clear();
        List<Hex> toAdd = new List<Hex>();
        foreach (Hex hex in areaForStep) //Toprak dışına 1 adım ilerleyebilmemiz için toprak ekler
        {
            if (GridSystem.AStar(startHex, hex, areaForStep).Count < stepAmount)
                if (GridSystem.AStar(startHex, hex, areaForStep).Count < stepAmount)
                    for (int i = 0; i < hex.neighbors.Count; i++)
                    {
                        if (hex.neighbors[i].Owner != ownedPlayer && !areaForStep.Contains(hex.neighbors[i]) && hex.neighbors[i]._hexType != hexType.water)
                        {
                            toAdd.Add(hex.neighbors[i]);
                        }
                    }
        }
        foreach (Hex hex in areaForStep)
        {
            hex.cost = 0;
            hex.estimatedCost = 0;
            hex.parent = null;
        }
        foreach (Hex hex in toAdd)
        {
            areaForStep.Add(hex);
        }
        toAdd.Clear();
        foreach (Hex hex in areaForStep)//Askerlerin yürüyemeceği toprakları çıkarır
        {
            foreach (Hex hex1 in hex.neighbors)
            {
                if (hex1.HexObjectType >= soldierLevel && (int)soldierLevel != (int)ObjectType.SoldierLevel4 && hex1.Owner != _owner && hex1.Owner == hex.Owner)
                {
                    toRemove.Add(hex);
                }
            }
            if ((int)hex.HexObjectType >= (int)soldierLevel && (int)soldierLevel != (int)ObjectType.SoldierLevel4 && hex.Owner != _owner)
            {
                toRemove.Add(hex);
            }

        }
        foreach (Hex hex in areaForStep)
        {
            if (hex.Owner == _owner && hex.HexObjectType != ObjectType.None && hex.HexObjectType != ObjectType.Tree && hex.HexObjectType != ObjectType.TreeWeak)
            {
                toRemove.Add(hex);
            }
        }

        foreach (Hex hex in toRemove)
        {
            areaForStep.Remove(hex);
        }
        toRemove.Clear();
        return areaForStep;
    }

    public void activateIndicator(bool request) //Hexlerin indicatorunu active/deactive eder
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(request);
    }

    public void destroyObjectOnHex()
    { //Hexin üstündeki objeyi yok eder
        if (ObjectOnHex != null)
        {
            Destroy(ObjectOnHex);
        }
        ObjectOnHex = null;
    }

    private void OnDestroy() {
        destroyObjectOnHex();
    }
}