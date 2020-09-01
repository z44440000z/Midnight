using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    LongRange,
    NearRange
}
public enum AIState
{
    RandomMoving,
    Track,
    Attack,
    Avoid
}

public class AI : MonoBehaviour
{
    public EnemyType _type;
    public AIState Status = new AIState { };

    //private RandomMove _randommove;

    private Track _track;

    //private Enemy_Attack _attack;

    public Animator _animator;

    public Transform _player;

    public float randommove_distance;
    public float track_distance;
    public float attack_distance;

    public bool sight;
    private float distance;
    public bool isHurt;

    private ParticleSystem Particle_hit;
    private Enemy_Health eHP;


    // Use this for initialization
    void Start()
    {
        //_randommove = GetComponent<RandomMove>();
        _track = GetComponent<Track>();
        //_attack = GetComponent<Enemy_Attack>();
        _player = GameObject.FindWithTag("Player").transform;
        Particle_hit = GetComponentInChildren<ParticleSystem>();
        eHP = GetComponent<Enemy_Health>();
    }

    // Update is called once per frame
    void Update()
    {
        Animation();
        Lineofsight();
        if (eHP._health <= 0)
        {
            dead();
            ParticleSystem _dieparticle = (ParticleSystem)Instantiate(eHP.Die_particle, transform);
            _dieparticle.Play();
        }

        distance = Vector3.Distance(_player.position, transform.position);

        switch (_type)
        {
            case EnemyType.LongRange:
                {
                    switch (Status)
                    {
                        case AIState.RandomMoving:
                            {
                                //_randommove.enabled = true;
                                if (distance >= attack_distance && distance < randommove_distance && sight)
                                {
                                    Status = AIState.Attack;
                                }
                                break;
                            }
                        case AIState.Attack:
                            {
                                if (!isHurt)
                                {
                                    // _randommove.enabled = false;
                                    // _randommove.Rotation_To(_player.position);
                                    //_attack.attack(1);
                                }
                                if (distance < attack_distance)
                                {
                                    //_attack.attack(0);
                                    Status = AIState.Avoid;
                                }
                                if (distance >= randommove_distance || !sight)
                                {
                                    //_attack.attack(0);
                                    Status = AIState.RandomMoving;
                                }
                            }
                            break;
                        case AIState.Avoid:
                            {
                                // _randommove.enabled = false;
                                // _randommove.Avoid(_player.position, attack_distance);
                                if (distance >= attack_distance && distance < randommove_distance && sight)
                                {
                                    Status = AIState.Attack;
                                }
                                if (distance >= randommove_distance || !sight)
                                {

                                    Status = AIState.RandomMoving;
                                }
                            }
                            break;

                    }

                }
                break;
            case EnemyType.NearRange:
                {
                    switch (Status)
                    {
                        case AIState.RandomMoving:
                            {
                                // _randommove.enabled = true;
                                if (distance <= track_distance && sight)
                                {
                                    _track.work();
                                    Status = AIState.Track;
                                }
                            }
                            break;
                        case AIState.Track:
                            {
                                // _randommove.enabled = false;
                                _track.Track_player(_player);
                                if (distance <= attack_distance)
                                {
                                    Status = AIState.Attack;
                                }
                                if (distance > randommove_distance || !sight)
                                {
                                    _track.stop();
                                    Status = AIState.RandomMoving;
                                }
                            }
                            break;
                        case AIState.Attack:
                            {
                                // _randommove.enabled = false;
                                // _randommove.Rotation_To(_player.position);
                                // _attack.attack(2);

                                if (distance > attack_distance)
                                {
                                    _track.work();
                                    Status = AIState.Track;
                                }
                            }
                            break;
                    }

                }
                break;

        }

    }

    void Animation()
    {
        switch (Status)
        {
            case AIState.RandomMoving:
                {
                    _animator.SetBool("idle", true);
                    _animator.SetBool("attack", false);
                }
                break;
            case AIState.Track:
                {
                    _animator.SetBool("idle", true);
                    _animator.SetBool("attack", false);
                }
                break;
            case AIState.Attack:
                {
                    _animator.SetBool("idle", false);
                    _animator.SetBool("attack", true);
                }
                break;
            case AIState.Avoid:
                {
                    _animator.SetBool("idle", true);
                    _animator.SetBool("attack", false);
                }
                break;
        }
    }

    public void Hurt(float d)
    {
        StartCoroutine( stopattack());
        eHP._health -= d;
        _animator.Play("M_hurt");
        Particle_hit.Play(true);
    }

    IEnumerator stopattack()
    {
        isHurt = true;
        yield return new WaitForSeconds(1.8f);
        isHurt = false;
    }

    void dead()
    {
        _animator.Play("M_die");
        eHP._health = 0;
        if (eHP._health == 0)
        {
            Destroy(this.gameObject, eHP.Die_time);
            //Destroy(eHP._fall, eHP.Die_time);
        }
    }

    void Lineofsight()
    {
        RaycastHit hit;
        Vector3 _direction = _player.position - transform.position;
        if (Physics.Raycast(transform.position, _direction, out hit))
        {
            if (hit.collider.tag == "Player")
            { sight = true; }
            else if (hit.collider.tag == "NPC")
            { sight = true; }
            else
            { sight = false; }
        }
        else
        { sight = false; }
    }
}
