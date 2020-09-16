using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_State : MonoBehaviour
{
    public bool is_move = false;
    public bool is_grounded = true;
    public bool is_lock;
    public bool is_hurt;

    public bool can_attack;
    public bool is_onstair;
    public bool is_debuff;

    public static bool ismove;
    public static bool isgrounded;
    public static bool islock;
    public static bool ishurt;
    public static bool canattack;
    public static bool isonstair;
    public static bool isdebuff;



    // Use this for initialization
    void Start()
    { }

    // Update is called once per frame
    void Update()
    {
        is_move = ismove;
        is_grounded = isgrounded;
        is_lock = islock;
        is_hurt = ishurt;
        can_attack = canattack;
        is_debuff = isdebuff;
    }

    public void Stun(float dur)
    {
        isdebuff = true;
        StartCoroutine(debuff_countdown(dur));
    }

    IEnumerator debuff_countdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isdebuff = false;
    }

}
