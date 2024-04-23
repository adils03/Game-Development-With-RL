using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierMovement : MonoBehaviour
{
    public GameObject soldier;
    List<Hex> walkableArea;
    public GameManager gameManager;
    Soldier soldierSc;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Walk();
        }

    }

    void Walk()
    {
        Vector2 soldierRay = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D soldierHit = Physics2D.Raycast(soldierRay, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Soldier"));
        if (soldierHit.collider != null)
        {
            GameObject hitObject = soldierHit.collider.gameObject;
            if (hitObject.tag == "Soldier")
            {
                HandleSoldierHit(hitObject);
            }
            else if (hitObject.tag == "Hex")
            {
                HandleHexHit(hitObject);
            }
            else
            {
                resetSoldierIndicators();
                ResetWalk();
            }
        }
        else
        {
            // Hexagon'a tıklanıp tıklanmadığını kontrol et
            RaycastHit2D hexHit = Physics2D.Raycast(soldierRay, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Hex"));
            if (hexHit.collider != null)
            {
                GameObject hitObject = hexHit.collider.gameObject;
                if (hitObject.tag == "Hex")
                {
                    HandleHexHit(hitObject);
                }
                else
                {
                    resetSoldierIndicators();
                    ResetWalk();
                }
            }
        }
    }

    void HandleSoldierHit(GameObject soldierHit)//Askere tıklamak için
    {
        ResetWalk();
        soldier = soldierHit;
        soldierSc = soldierHit.GetComponent<Soldier>();
        resetSoldierIndicators();
        if (gameManager.GetTurnPlayer() == soldierSc.owner && !soldierSc.hasMoved)
        {
            walkableArea = soldierSc.onHex.travelContinentByStepForSoldier(4, soldierSc.owner, soldierSc.soldierLevel);
            soldierSc.activateIndicator(true);
            foreach (Hex hex in walkableArea)
            {
                hex.activateIndicator(true);
            }
        }
    }

    void HandleHexHit(GameObject hexHit)//Hexe tıklamak için
    {
        Hex hexComponent = hexHit.GetComponent<Hex>();
        if (walkableArea != null && walkableArea.Contains(hexComponent))
        {
            ProcessValidHex(hexComponent);
        }
        else
        {
            resetSoldierIndicators();
            ResetWalk();
        }
    }

    void ProcessValidHex(Hex hex) //Askerin yürüdüğü toprağın parametrelerini ayarlar 
    {
        soldierSc = soldier.GetComponent<Soldier>();
        soldierSc.walkToHex(hex);
        ResetWalk();
        resetSoldierIndicators();
    }


    void ResetWalk()
    {
        if (walkableArea != null)
        {
            foreach (Hex hex in walkableArea)
            {
                hex.activateIndicator(false);
            }
            walkableArea = null;
        }
    }

    void resetSoldierIndicators()
    {
        if (soldierSc != null)
        {
            foreach (Soldier _soldier in soldierSc.owner.soldiers)
            {
                _soldier.activateIndicator(false);
            }
        }

    }
}
