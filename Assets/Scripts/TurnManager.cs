using System.Collections.Generic;
using UnityEngine;

//Oyuncuların sırasını yönetir.Players adında bir liste ve turnQueue adında bir kuyruk içerir.
public class TurnManager
{
    public List<Player> players;

    public Queue<Player> turnQueue = new Queue<Player>();

    public Player currentPlayer;


    public TurnManager(List<Player> players)//Oyuncu listesini alarak turnManager nesnesi oluşturur ve ilk turu başlatır
    {
        this.players = players;
        foreach (Player player in players)
        {
            turnQueue.Enqueue(player);
        }

    }
    
    //StartTurn metodu, bir oyuncunun sırasını başlatır. Eğer tüm oyuncuların sırası biterse, kuyruğu yeniden doldurur.
    public void StartTurn()
    {
        if (turnQueue.Count == 0)
        {
            foreach (Player player in players)
            {
                turnQueue.Enqueue(player);
            }
        }

        Player currentPlayer = turnQueue.Dequeue();

        currentPlayer.StartTurn();
    }

    //EndTurn metodu geçerli oyuncunun sırasını bitirir ve bir sonraki oyuncunun sırasını başlatır.
    public void EndTurn()
    {
        currentPlayer = null;
    }

    //sirayi bitirir
    public void passTurn()
    {
        if (currentPlayer == null)
        {
            StartTurn();
        }
    }
}