using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class Player_Animator : MonoBehaviour
{
    public Animator _animator;
    public Player_State _playerState;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _playerState = transform.parent.GetComponent<Player_State>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") != 0)
        { _animator.SetBool("run", true); }
        else if (Input.GetAxis("Horizontal") != 0)
        { _animator.SetBool("run", true); }
        else
        { _animator.SetBool("run", false); }

        if (!_playerState.is_grounded)
        {
            _animator.Play("NP_Jump");
            _animator.SetBool("jump", true);
        }

        if (_playerState.is_grounded)
        { _animator.SetBool("jump", false); }

    }
}
