using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string playerName;
    public int PlayerTotalGold = 50;// player altını burdan da görüyoruz kaynak eManager
    public List<Hex> ownedHexes = new List<Hex>(); //sahip olduğu hexler
    public List<Soldier> soldiers = new List<Soldier>();
    public EconomyManager economyManager = new EconomyManager();
    public Color playerColor;
    public bool isAI = false;
    public Player(String name)// bu ctor diğerleri patlamasın diye geçici duruyor daha karar verilmedi
    {
        playerName = name;

    }
    public Player(String name, List<Hex> hexes, Color color)// dışardan gelen ilk hexlerle ve oyun boyunca elde edilecek olan hexler için inşa edilecek olan kısım
    {
        playerColor = color;
        ownedHexes = hexes;
        // Hex'lerin sahibini bu oyuncu olarak ayarla
        foreach (var hex in ownedHexes)
        {
            hex.playerName = name;
            hex.Owner = this;
            hex.gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        }


        playerName = name;
        economyManager.UpdateOwnedHexagons(ownedHexes);

    }
    public void PlayerUpdate(List<Hex> hexes, Color color)
    {
        playerColor = color;
        ownedHexes = hexes;
        // Hex'lerin sahibini bu oyuncu olarak ayarla
        foreach (var hex in ownedHexes)
        {
            hex.Owner = this;
            hex.playerName = playerName;
            hex.gameObject.GetComponent<SpriteRenderer>().color = playerColor;
        }
        economyManager.UpdateOwnedHexagons(ownedHexes);

    }
    public void EconomyDeath()// ekonomi çöktü askerler imha edildi
    {
        foreach (var hex in ownedHexes)
        {
            ObjectType s = hex.HexObjectType;
            if (s == ObjectType.SoldierLevel1 || s == ObjectType.SoldierLevel2 || s == ObjectType.SoldierLevel3 || s == ObjectType.SoldierLevel4)
            {
                hex.HexObjectType = ObjectType.None;
                //hex.ObjectOnHex = null; // bundan emin değilim sahibi baksun (ben ibo)
                hex.ObjectOnHex.GetComponent<Soldier>().isEconomyDeath = true;
                hex.destroyObjectOnHex();
                //Burdurda birde askerlerin yerine mezar gelmesi mantıklı olur 1 tur için 
            }
        }
        this.soldiers.Clear();
        PlayerTotalGold = 0;
        economyManager.UpdateOwnedHexagons(ownedHexes);// income tekrar hesaplansın
    }


    void UpdateTotalGold()
    {
        PlayerTotalGold += economyManager.totalIncome;
        if (PlayerTotalGold < 0)
        {
            EconomyDeath();
        }
    }
    public void StartTurn()
    {
        economyManager.UpdateOwnedHexagons(ownedHexes);// sahip olduğumuz hexleri ekonomi managerde güncelledik
        UpdateTotalGold();// burdan da tur başına güncelledik kasadaki altını
    }
    public void ChangeHexOwnership(Hex changedHex)
    {
        // EconomyManager'ın sahip olduğu hex listesini güncelle spesifik bir hex'e göre
        economyManager.HexOwnershipChanged(changedHex);
    }
    public void AddHex(Hex newHex)
    {
        if (newHex != null && !ownedHexes.Contains(newHex))
        {
            ownedHexes.Add(newHex);
            newHex.Owner = this;
        }
    }
    public void AddHex(List<Hex> newHexes)// çok mu çok tatlı
    {
        foreach (var newHex in newHexes)
        {
            AddHex(newHex);
        }
    }

    public void Death()//Ölen oyuncunun tüm her şeyi sıfırlanıyor
    {
        playerName = null;
        PlayerTotalGold = 0;
        Color32 playerColor = new Color32(0x2F, 0xBC, 0x0B, 0xFF);

        foreach (Hex hex in ownedHexes)
        {
            hex.Owner = null;
            hex.playerName = null;
            hex.gameObject.GetComponent<SpriteRenderer>().color = playerColor;
            if (hex.HexObjectType == ObjectType.TownHall || hex.HexObjectType == ObjectType.BuildingFarm || hex.HexObjectType == ObjectType.BuildingDefenceLevel1 || hex.HexObjectType == ObjectType.BuildingDefenceLevel2)
            {
                hex.destroyObjectOnHex();
            }

        }
        ownedHexes.Clear();
        foreach (Soldier soldier in soldiers)
        {
            soldier.owner = null;
            soldier.playerName = null;
            if (soldier.gameObject != null)
            {
                SpriteRenderer spriteRenderer = soldier.gameObject.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.sprite = null;
                }
            }
        }
        soldiers.Clear();
    }
}