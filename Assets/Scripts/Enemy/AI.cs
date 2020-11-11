using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    //private Enemy_Attack _attack;

    public Animator animator;
    public AudioSource audioSource;

    public Transform player;

    //扇形範圍區域探查距離
    public float probeDistance;
    //扇形範圍區域探查方向
    public float probeAngle;
    float distance;
    SimpleCharacterControl pc;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        pc = player.GetComponent<SimpleCharacterControl>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, player.position);
        if (distance < probeDistance)
        {
            if (CheackScope(transform.position, player.position))
            {
                if (pc.m_State.IsName("Base Layer.Climb.Climbing") ||pc.m_State.IsName("Base Layer.Climb.ClimbUp") || pc.m_State.IsName("Base Layer.Climb.ClimbDown"))
                { }
                else
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                        pc.PlayerControl(false);
                        StartCoroutine("Timer");
                    }

                }
            }
            else
            {
                Debug.Log("沒進入檢測範圍");
            }
        }
        else
        {
        }
    }

    private bool CheackScope(Vector3 _avatarPos, Vector3 _enemyPos)
    {
        Vector3 srcVect = _enemyPos - _avatarPos;
        Vector3 fowardPos = transform.forward * 1 + _avatarPos;
        Vector3 fowardVect = fowardPos - _avatarPos;
        fowardVect.y = 0;
        float angle = Vector3.Angle(srcVect, fowardVect);
        if (angle < probeAngle / 2)
        { return true; }
        else
        { return false; }
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        pc.Dead();
    }
}
