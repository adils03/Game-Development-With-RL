using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Soldier : MonoBehaviour
{
    public ObjectType soldierLevel;
    public Hex onHex;
    public Player owner;
    public String playerName;
    public bool hasMoved = false;//Asker tur içinde yürüdü mü
    public bool isEconomyDeath = false;
    [SerializeField] GameObject grave;
    private void OnDestroy()
    {
        owner.soldiers.Remove(this);
        if(isEconomyDeath){
            spawnGrave();
        }
    }
    private void Start()
    {
        playerName = owner.playerName; 
    }

    private void Update() {
        if (transform.position != onHex.transform.position)
        {
            transform.position = Vector3.Lerp(transform.position, onHex.transform.position, 5 * Time.deltaTime);
        }
    }

    public void activateIndicator(bool request)
    {
        transform.GetChild(0).gameObject.SetActive(request);
    }

    public void walkToHex(Hex hex)
    {
        if (hex.HexObjectType == ObjectType.Tree || hex.HexObjectType == ObjectType.TreeWeak)
        {
            hex.destroyObjectOnHex();
            owner.PlayerTotalGold += 4;
        }
        else if (hex.ObjectOnHex != null)
        {
            hex.destroyObjectOnHex();
        }
        onHex.HexObjectType = ObjectType.None;
        onHex.ObjectOnHex = null;
        hex.ObjectOnHex = this.gameObject;
        hex.HexObjectType = soldierLevel;
        if (hex.Owner != null && owner != hex.Owner)
        {
            hex.Owner.ownedHexes.Remove(hex);
        }
        hex.Owner = owner;
        if (!hex.Owner.ownedHexes.Contains(hex))
        {
            hex.Owner.ownedHexes.Add(hex);
        }
        hex.playerName = playerName;
        hex.GetComponent<SpriteRenderer>().color = owner.playerColor;
        onHex = hex;
        hasMoved = true;
        hex.UpdateAdvantageOrDisadvantageValue();
    }

    void spawnGrave()
    {
        GameObject _grave = Instantiate(grave, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
        Grave _graveSc = _grave.GetComponent<Grave>();
        _graveSc.owner = owner;
        _graveSc.onHex = onHex;
        onHex.ObjectOnHex = _grave;
        onHex.HexObjectType = ObjectType.TreeWeak;
    }

}

