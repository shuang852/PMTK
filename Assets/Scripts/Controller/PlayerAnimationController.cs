using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void UpdateMoveAnimation(float moveY)
    {
        //_animator.SetFloat("Y", moveY);
        if (moveY > 0)
        {
            _animator.SetBool("MoveUp", true);
            _animator.SetBool("MoveDown", false);
        }
        else if (moveY < 0)
        {
            _animator.SetBool("MoveDown", true);
            _animator.SetBool("MoveUp", false);
        }
        else
        {
            _animator.SetBool("MoveDown", false);
            _animator.SetBool("MoveUp", false);
        }
    }
}