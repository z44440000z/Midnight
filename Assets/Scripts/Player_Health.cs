using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    public delegate void addHP(float f);
    public addHP addPHP;

	  public static GameObject _player;

    /*------------------------------------------*/
    public int MaxHP;
    public float NowHP;
    public int MaxMP;

    [SerializeField]
    public float NowMP;
    public float SecMP;
    public bool isDead = false;
    public bool isInvincibility = false;
    public GameObject UIPrefab_ShowDamage;

    // Use this for initialization
    void Start()
    {
        NowHP = MaxHP;
        NowMP = MaxMP;
        addPHP = AddHP;
    }

    // Update is called once per frame
    void Update()
    {
		//F3無敵
        if (Input.GetKeyDown(KeyCode.F3))
        { isInvincibility = !isInvincibility; }

        if (isInvincibility)
        {
            NowHP = MaxHP;
            NowMP = MaxMP;
        }

        if (!isDead)
        {
            /*HP*/
            if (NowHP <= 0)
            {
                NowHP = 0;
                isDead = true;
            }
            /*MP*/
            if (NowMP < MaxMP)
            {
                //if (ComboSystem && ComboSystem.CanCombo)
                NowMP += Time.deltaTime * SecMP;
            }
            else
            {
                NowMP = (float)MaxMP;
            }
        }
        else
        {
            NowHP = 0;
            NowMP = 0;
        }
    }

    public void SetHP(int damage)
    {
        NowHP -= damage;
    }
    public void SetMP(float mana)
    {
        NowMP += mana;
    }
    public float GetHPPercent()
    {
        return ((float)NowHP / (float)MaxHP) * 100;
    }

    private void AddHP(float reply)
    {
        NowHP += reply;
    }
}
