using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour
{
    public Player owner;
    public Hex onHex;
    GameManager gameManager;
    [SerializeField] Sprite sprite;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (gameManager.GetTurnPlayer() == owner)
        {
            onHex.Income = 0;
            onHex.HexObjectType = ObjectType.TreeWeak;
            spriteRenderer.sprite = sprite;
        }
    }
}
