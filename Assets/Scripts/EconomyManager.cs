using System;
using System.Collections.Generic;
using UnityEngine;

public class EconomyManager
{

    public int totalIncome;
    private List<Hex> OwnedHexagons = new List<Hex>();

    public EconomyManager()
    {
      
        if( OwnedHexagons != null )
        {
            CalculateIncome();
        }
    }
    /// <summary>
    /// Devlet gelirini hesaplar maaşları çıkarır ve geriye net geliri döndürür
    /// Yapay Zeka yazarkene bunu çokça yapalım
    /// </summary>
    /// <returns>Oyuncu Geliri(int)</returns>
    public int CalculateIncome()
    {     
        totalIncome = 0;
        foreach (Hex hexagon in OwnedHexagons)
        {
            totalIncome += hexagon.Income;
            
        }
        CalculateSalaries();
        return totalIncome;
    }

    void CalculateSalaries()// maaş bilgilerini totalIncomedan yaniii gelirden düştük
    {
        foreach (Hex hex in OwnedHexagons)
        {
            int salarySoldier = 0;

                 if (hex.HexObjectType == ObjectType.SoldierLevel1) { salarySoldier =  2; }
            else if (hex.HexObjectType == ObjectType.SoldierLevel2) { salarySoldier = 6; }
            else if (hex.HexObjectType == ObjectType.SoldierLevel3) { salarySoldier = 18; }
            else if (hex.HexObjectType == ObjectType.SoldierLevel4) { salarySoldier = 36; }
            else if (hex.HexObjectType == ObjectType.BuildingDefenceLevel1) { salarySoldier = 1; }// çokta abartmayalım hard oluyor
            else if (hex.HexObjectType == ObjectType.BuildingDefenceLevel2) { salarySoldier = 6; }// yeterli

            totalIncome -= salarySoldier;
        }

    }
    public void UpdateOwnedHexagons(List<Hex> newHexagons)
    {
        OwnedHexagons = newHexagons;
        CalculateIncome();
       
           
    }
   
    public void HexOwnershipChanged(Hex changedHex)
    {   
        // spesifik hex sahibi değişti bu metot çağıralabilir
        List<Hex> updatedHexagons = GetUpdatedHexagons(changedHex);

        // UpdateOwnedHexagons metodunu çağırarak ekonomiyi güncelle
        UpdateOwnedHexagons(updatedHexagons);
    }    
    private List<Hex> GetUpdatedHexagons(Hex changedHex)
    {
        // Hex'in sahipliği değiştiğinde çağrılacak yardımcı bir metod
        List<Hex> updatedHexagons = new List<Hex>(OwnedHexagons);

        // Değişen hex'i listeden kaldır veya ekleyebilirsiniz
        if (updatedHexagons.Contains(changedHex))
        {
            updatedHexagons.Remove(changedHex);
        }
        else
        {
            updatedHexagons.Add(changedHex);
        }

        return updatedHexagons;
    }

}




