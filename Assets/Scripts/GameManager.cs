using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public List<String> playerNames = new List<string>();
    public List<Player> players = new List<Player>();
    public List<Hex> hexes;
    private TurnManager turnManager;
    private GridSystem gridSystem;
    private Player player;
    private SpawnManager spawnManager;
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI player1Gold;
    [SerializeField] private TextMeshProUGUI player2Gold;
    [SerializeField] private TextMeshProUGUI player3Gold;
    [SerializeField] private bool canStartGame = false;
    private void Awake()
    {
        gridSystem = GameObject.Find("GridSystem").GetComponent<GridSystem>();
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        this.hexes = gridSystem.hexes;


    }
    private void Start()
    {
        if (canStartGame)
        {
            RestartGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
        updatePlayerGoldAndIncome();
        CheckPlayerTown(players, spawnManager.spawnedHouses);
    }
    
    public void endTurn() //Buton ataması için konulmuştur.
    {
        turnManager.StartTurn();
        DeathCheck();
        EconomyDeathCheck();
        resetSoldierMoves();
        turnText.text = "Turn: " + GetTurnPlayer().playerName;
        if (turnManager.turnQueue.Count == 0)
        {
            spawnManager.TreeSpreadWithOrder();
        }
    }

    public Player GetTurnPlayer()//Sıra hangi oyuncudaysa onu döndürür
    {
        if (turnManager.turnQueue.Count == 0)
        {
            return players[0];
        }
        return turnManager.turnQueue.Peek();
    }

    void CheckPlayerTown(List<Player> players, List<Hex> spawnedHouses)//playerın evi yıkıldı mı yıkılmadı mı onu kontrol ediyo sürekli kontrol etmeyi update yazdım 
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            Hex playerHex = spawnedHouses[i];

            if (playerHex.HexObjectType != ObjectType.TownHall)
            {
                player.Death();
            }
        }
    }

    void DeathCheck()// Parası 0, income 0 ve hiç askeri olmadığında ölmesini sağlıyor
    {
        Player currentPlayer = GetTurnPlayer();

        if (currentPlayer.PlayerTotalGold == 0 && currentPlayer.soldiers.Count == 0 && currentPlayer.economyManager.totalIncome == 0)
        {
            currentPlayer.Death();
            if (turnManager.turnQueue.Count > 0)
            {
                turnManager.turnQueue.Dequeue();
            }
            Debug.Log(turnManager.turnQueue.Count);
        }
    }

    void EconomyDeathCheck()
    {
        Player currentPlayer = GetTurnPlayer();

        if (currentPlayer.PlayerTotalGold < 0 && currentPlayer.economyManager.CalculateIncome() < 0)
        {
            currentPlayer.EconomyDeath();
        }
    }
    void resetSoldierMoves()
    {//Tur bitince askerlerin yürümülerini sıfırlamak için
        foreach (Player player in players)
        {
            foreach (Soldier soldier in player.soldiers)
            {
                soldier.hasMoved = false;
                if (GetTurnPlayer() != player)
                {
                    soldier.GetComponent<CircleCollider2D>().enabled = false;
                }
                else
                {
                    soldier.GetComponent<CircleCollider2D>().enabled = true;
                }
            }
        }
    }
    void updatePlayerGoldAndIncome()
    {//Ekranda oyuncuların toplam paralarını ve gelirlerinin atamasını yapar
        player1Gold.text = players[0].playerName + ": " + players[0].PlayerTotalGold + "(" + players[0].economyManager.CalculateIncome() + ")";
        player2Gold.text = players[1].playerName + ": " + players[1].PlayerTotalGold + "(" + players[1].economyManager.CalculateIncome() + ")";
        player3Gold.text = players[2].playerName + ": " + players[2].PlayerTotalGold + "(" + players[2].economyManager.CalculateIncome() + ")";
    }
   
    public void RestartGame()
    {   
        Debug.Log("Game Restarted");
        gridSystem.CreateGrid(gridSystem.size);
        players.Clear();
        players.Add(new Player("Burak"));
        players.Add(new Player("Halil"));
        players.Add(new Player("Adil"));

        turnManager = new TurnManager(players);
        spawnManager.spawnedHouses.Clear();

        spawnManager.SpawnLandOfPlayers(gridSystem.size, players);
        spawnManager.SpawnTreesOnSpecificHexes(spawnManager.treeCoordinatesQ,spawnManager.treeCoordinatesR);

        turnText.text = "Turn: " + turnManager.players[0].playerName;
    }
}
