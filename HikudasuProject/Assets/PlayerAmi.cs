
using UnityEngine;

public class PlayerAmi : MonoBehaviour
{
    private Animator animator;
    private Transform _transform;
    void Start()
    {
        animator = GetComponent<Animator>();
        _transform = transform;
    }
    void Update()
    {
        var W = Input.GetKeyDown(KeyCode.W);
        var A = Input.GetKeyDown(KeyCode.A);
        var S = Input.GetKeyDown(KeyCode.S);
        var D = Input.GetKeyDown(KeyCode.D);
        float transPosX = transform.position.x;
        float transPosZ = transform.position.z;
        if(W || A || S || D )
        {
            animator.Play("Walk");
            if(D)
            {
                _transform.localRotation = new Quaternion(transPosX, 90, transPosZ, 0);
                Debug.Log(D);
            }
            else if (A)
            {
                //_transform.localRotation = new Quaternion(transPosX, -90, transPosZ, 0);
                Debug.Log(A);
            }
            else if (S)
            {
                //_transform.localRotation = new Quaternion(transPosX, 180, transPosZ, 0);
                Debug.Log(S);
            }
        }
    }
}
