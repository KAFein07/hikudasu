using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 10f;
    public float jumpPower = 8.0f;

    void Update()
    {
        // “ü—ÍŽæ“¾
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // ˆÚ“®ˆ—
        Vector3 moveDirection = new Vector3(x, 0, z);
        transform.position += moveDirection;

        // ƒWƒƒƒ“ƒvˆ—
        if (Input.GetButton("Jump"))
        {
            moveDirection.y = jumpPower;
        }

        // ŠŠ‚ç‚©‚ÈŒü‚«•ÏX
        if (moveDirection.magnitude > 0)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }
    }
}