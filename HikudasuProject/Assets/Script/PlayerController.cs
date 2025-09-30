using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private GameObject lowLight;
    [SerializeField] private GameObject resultText;

    private float moveSpeed = 5;
    private float rotateSpeed = 10;
    private float jumpPower = 250;
    private float horizontalInput;
    private float verticalInput;
    private bool downInput;
    private bool canJump;
    private Vector3 moveDirection;
    private Vector3 rotation;
    private Rigidbody rb;

    private GameObject[] allStageBlocks;
    public List<GameObject> selectedObjects = new List<GameObject>();

    //public float blockPosX;
    //public float blockPosY;
    //public float blockPosZ;

    public bool canCatch = false;
    public int view = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        allStageBlocks = GameObject.FindGameObjectsWithTag("Stage");
    }

    private void Update()
    {
        //Debug.Log("canJump=" + canJump);

        // 入力取得
        switch (view)
        {
            case 1:
                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
                break;
            case 2:
                horizontalInput = Input.GetAxis("Vertical");
                verticalInput = -Input.GetAxis("Horizontal");
                break;
            case 3:
                horizontalInput = -Input.GetAxis("Horizontal");
                verticalInput = -Input.GetAxis("Vertical");
                break;
            case 4:
                horizontalInput = -Input.GetAxis("Vertical");
                verticalInput = Input.GetAxis("Horizontal");
                break;
        }
        downInput = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
        //Debug.Log(verticalInput);
        //Debug.Log(horizontalInput);

        // 移動処理
        moveDirection = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, verticalInput * moveSpeed * Time.deltaTime);
        transform.position += moveDirection;

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector3.up * jumpPower);
            canJump = false;
        }

        // 滑らかな向き変更
        if (horizontalInput != 0 || verticalInput != 0)
        {
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
        }

        //視点変更
        //*
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(RotationCoroutine(1));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(RotationCoroutine(-1));
        }//*/

        //if (selectedObjects.Count > 0 && verticalInput < 0)
        if (selectedObjects.Count > 0 && downInput)
        {
            //Debug.Log("copy");
            foreach (var obj in selectedObjects)
            {
                GameObject copyObject = Instantiate(obj);
                Vector3 pos = copyObject.transform.position;
                switch (view)
                {
                    case 1:
                        pos.z -= 1;
                        break;
                    case 2:
                        pos.x -= 1;
                        break;
                    case 3:
                        pos.z += 1;
                        break;
                    case 4:
                        pos.x += 1;
                        break;
                }
                copyObject.transform.position = pos;
                //copyObject.GetComponent<StageBlockController>().ColorChange();
            }
            canCatch = false;
            Debug.Log("listClear");
            selectedObjects.Clear();
        }
    }

    public void ObjectSelect(int ID)
    {
        for (int i = 0; i < allStageBlocks.Length; i++)
        {
            int _blockID = allStageBlocks[i].GetComponent<StageBlockController>().blockID;
            if (_blockID == ID)
            {
                selectedObjects.Add(allStageBlocks[i]);
            }
        }
    }


    private IEnumerator RotationCoroutine(int rotationDirection)
    {
        rotation = cameraCenter.eulerAngles;
        for (int i = 0; i < 90; i++)
        {
            rotation.y += 1 * rotationDirection;
            cameraCenter.eulerAngles = rotation;
            yield return null;
        }
        if (rotationDirection == 1)
        {
            view += 1;
        }
        else
        {
            view -= 1;
        }
        if (view == 0)
        {
            view = 4;
        }
        if (view == 5)
        {
            view = 1;
        }
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            canJump = true;
        }
        if (collision.gameObject.tag == "Stage")
        {
            canJump = true;
            canCatch = true;
        }
        if (collision.gameObject.tag == "Flag")
        {
            lowLight.SetActive(true);
            resultText.SetActive(true);
            this.enabled = false;
        }
        Debug.Log(collision .gameObject.name);
    }
    /*
    public void PositionCheck(Vector3 pos)
    {
        if (view == 1 || view == 3)
        {
            blockPosZ = pos.z;
            blockPosY = pos.y;
        }
        else
        {
            blockPosX = pos.x;
            blockPosY = pos.y;
        }
    }

    public void PositionNull()
    {
        blockPosX = 0;
        blockPosY = 0;
        blockPosZ = 0;
    }//*/
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            canCatch = false;
        }
    }
}