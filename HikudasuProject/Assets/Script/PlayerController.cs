using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private GameObject lowLight;
    [SerializeField] private GameObject resultText;
    public Material[] yellowMaterials;
    public Material[] redMaterials;

    private float moveSpeed = 5;
    private float rotateSpeed = 10;
    private float jumpPower = 250;
    private float horizontalInput;
    private float verticalInput;
    private float scrollInput;
    private bool canMove = true;
    private bool canRotate = true;
    private bool canJump;
    private Vector3 moveDirection;
    [SerializeField] private int catchBlockID = -1;
    private int maxBlockID = 0;
    private float zoomSpeed = 1000;
    private int childCount;
    private Rigidbody rb;
    private GameObject camera;
    private List<GameObject> allStageBlocks;
    public List<GameObject> selectedObjects = new List<GameObject>();
    public bool canCatch = false;
    public int view = 1;

    [SerializeField] private bool catchMode;
    private bool _bool = false;
    private bool onlyPlus = false;
    private bool onlyMinus = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera = Camera.main.gameObject;
        scrollInput = Input.GetAxis("Mouse ScrollWheel");
        GameObject[] _allStageBlocks = GameObject.FindGameObjectsWithTag("Stage");
        allStageBlocks = new List<GameObject>(_allStageBlocks);
        foreach (var obj in allStageBlocks)
        {
            int _blockID = obj.GetComponent<StageBlockController>().blockID;
            if (maxBlockID <= _blockID)
                maxBlockID = _blockID;
        }
    }

    private void Update()
    {
        childCount = transform.childCount;

        // 入力取得
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

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
        if (catchMode)
        {
            if (view == 1 || view == 3)
            {
                horizontalInput = 0;
                if (onlyPlus && verticalInput > 0)
                    verticalInput = 0;
                else if (onlyMinus && verticalInput < 0)
                    verticalInput = 0;
            }
            else
            {
                verticalInput = 0;
                if (onlyPlus && horizontalInput > 0)
                    horizontalInput = 0;
                else if (onlyMinus && horizontalInput < 0)
                    horizontalInput = 0;
            }
        }

        if (catchMode)
            moveSpeed = 2.5f;
        else
            moveSpeed = 5f;

        // 移動処理
        if (canMove)
        {
            moveDirection = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, verticalInput * moveSpeed * Time.deltaTime);
            transform.position += moveDirection;
        }
        camera.transform.position += scrollInput * camera.transform.forward * zoomSpeed * Time.deltaTime;

        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(Vector3.up * jumpPower);
            canJump = false;
        }

        // 滑らかな向き変更
        if ((horizontalInput != 0 || verticalInput != 0) && (catchMode == false))
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

        //視点変更
        if (Input.GetKeyDown(KeyCode.E) && canRotate)
        {
            StartCoroutine(RotationCoroutine(1));
            canRotate = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) && canRotate)
        {
            StartCoroutine(RotationCoroutine(-1));
            canRotate = false;
        }

        //if (selectedObjects.Count > 0 && verticalInput < 0)
        if (selectedObjects.Count > 0 && Input.GetMouseButtonDown(1))
        {
            foreach (var obj in selectedObjects)
            {
                Vector3 objPos = obj.transform.position;
                GameObject copyObject = Instantiate(obj, objPos, obj.transform.rotation,  this.transform);
                StageBlockController _stageBlockController = copyObject.GetComponent<StageBlockController>();
                _stageBlockController.Initialize(this, objPos, view, maxBlockID);
                allStageBlocks.Add(copyObject);
            }
            selectedObjectsClear();
            StartCoroutine(CatchModeCoroutine());
        }

        if (canCatch && Input.GetMouseButtonDown(0) && (catchMode == false))
        {
            ObjectSelect(catchBlockID);
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedObjectsClear();
        }
    }

    private void selectedObjectsClear()
    {
        foreach (var obj in selectedObjects)
            obj.GetComponent<StageBlockController>().ChangeColorHigh();
        selectedObjects.Clear();
    }

    private void ObjectSelect(int ID)
    {
        foreach (var obj in allStageBlocks)
        {
            int _blockID = obj.GetComponent<StageBlockController>().blockID;
            if (_blockID == ID)
            {
                selectedObjects.Add(obj);
                obj.GetComponent<StageBlockController>().ChangeColorLow();
            }
        }
    }

    private IEnumerator CatchModeCoroutine()
    {
        while (true)
        {
            yield return null;
            catchMode = true;
            bool DoOnce = true;
            if (DoOnce)
            {
                rb.isKinematic = true;
                DoOnce = false;
            }
            if (childCount <= 1)
            {
                rb.isKinematic = false;
                catchMode = false;
            }
            if (catchMode == false)
            {
                catchBlockID = -1;
                yield break;
            }
        }
    }

    private IEnumerator RotationCoroutine(int rotationDirection)
    {
        Vector3 rotation = cameraCenter.eulerAngles;
        for (int i = 0; i < 90; i++)
        {
            rotation.y += 1 * rotationDirection;
            cameraCenter.eulerAngles = rotation;
            yield return null;
        }
        if (rotationDirection == 1)
            view += 1;
        else
            view -= 1;
        if (view == 0)
            view = 4;
        if (view == 5)
            view = 1;
        canRotate = true;
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
            canJump = true;
        if (collision.gameObject.tag == "Stage")
        {
            float posY = transform.position.y;
            float blockPosY = collision.transform.position.y;
            float diff = Mathf.Abs(blockPosY - posY);
            if (diff <= 0.1f)
            {
                catchBlockID = collision.gameObject.GetComponent<StageBlockController>().blockID;
                canCatch = true;
            }
            else if (blockPosY < posY)
                canJump = true;
        }
        if (collision.gameObject.tag == "Flag")
        {
            lowLight.SetActive(true);
            resultText.SetActive(true);
            this.enabled = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stage")
        {
            catchBlockID = -1;
            canCatch = false;
        }
    }
}