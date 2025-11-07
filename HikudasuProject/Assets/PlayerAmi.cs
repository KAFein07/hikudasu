
using UnityEngine;

public class PlayerAmi : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.Play("Walk");
        }
    }
}
