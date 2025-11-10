using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private GameObject lowLight;
    [SerializeField] private GameObject resultText;

    public Material[] yellowMaterials;
    public Material[] redMaterials;
    public List<GameObject> selectedObjects = new List<GameObject>();
    public bool canCatch = false;
    public int view = 1;

    private float moveSpeed = 5;
    private float rotateSpeed = 10;
    private float jumpPower = 250;
    private float horizontalInput;
    private float verticalInput;
    private float scrollInput;
    private float zoomSpeed = 1000;
    private bool canMove = true;
    private bool canRotate = true;
    private bool canJump;
    private bool catchMode;
    private int catchBlockID = -1;
    private int maxBlockID = 0;
    private int moveVector;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private GameObject camera;
    private List<GameObject> allStageBlocks;

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
        maxBlockID *= 4;
    }

    private void Update()
    {
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
        if (moveVector == 1)
        {
            switch (view)
            {
                case 1:
                    if (verticalInput < 0)
                        verticalInput = 0;
                    break;
                case 2:
                    if (horizontalInput < 0)
                        horizontalInput = 0;
                    break;
                case 3:
                    if (verticalInput > 0)
                        verticalInput = 0;
                    break;
                case 4:
                    if (horizontalInput > 0)
                        horizontalInput = 0;
                    break;
            }
        }
        else if (moveVector == -1)
        {
            switch (view)
            {
                case 1:
                    if (verticalInput > 0)
                        verticalInput = 0;
                    break;
                case 2:
                    if (horizontalInput > 0)
                        horizontalInput = 0;
                    break;
                case 3:
                    if (verticalInput < 0)
                        verticalInput = 0;
                    break;
                case 4:
                    if (horizontalInput < 0)
                        horizontalInput = 0;
                    break;
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
        if (Input.GetKeyDown(KeyCode.E) && canRotate && catchMode == false)
        {
            StartCoroutine(RotationCoroutine(1));
            canRotate = false;
        }
        if (Input.GetKeyDown(KeyCode.Q) && canRotate && catchMode == false)
        {
            StartCoroutine(RotationCoroutine(-1));
            canRotate = false;
        }

        if (selectedObjects.Count > 0 && Input.GetMouseButtonDown(1))
        {
            var grouped = selectedObjects.GroupBy(b => Mathf.Round(b.transform.position.y * 100f) / 100f).ToList();
            List<GameObject> result = new List<GameObject>();
            foreach (var group in grouped)
            {
                if (view == 1 || view == 3)
                {
                    float minDistance = group.Min(b => Mathf.Abs(b.transform.position.z - this.transform.position.z));
                    var nearestBlocks = group.Where(b => Mathf.Abs(b.transform.position.z - this.transform.position.z) == minDistance).ToList();
                    result.AddRange(nearestBlocks);
                }
                else
                {
                    float minDistance = group.Min(b => Mathf.Abs(b.transform.position.x - this.transform.position.x));
                    var nearestBlocks = group.Where(b => Mathf.Abs(b.transform.position.x - this.transform.position.x) == minDistance).ToList();
                    result.AddRange(nearestBlocks);
                }
            }
            selectedObjects = result;
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

        //Debug.Log(Input.GetAxis("Vertical"));
    }

    private void selectedObjectsClear()
    {
        foreach (var obj in allStageBlocks)
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
        int maxCopylevel = 0;
        int minCopylevel = 3;

        foreach (var obj in selectedObjects)
        {
            int _copylevel = obj.GetComponent<StageBlockController>().copyLevel;
            if (maxCopylevel <= _copylevel)
                maxCopylevel = _copylevel;
            if (minCopylevel >= _copylevel)
                minCopylevel = _copylevel;
        }

        while (true)
        {
            yield return null;
            if (Input.GetAxis("Vertical") < 0 && selectedObjects.Count > 0 && maxCopylevel != 3)
            {
                foreach (var obj in selectedObjects)
                {
                    GameObject copyObject = Instantiate(obj, obj.transform.position, obj.transform.rotation, this.transform);
                    copyObject.GetComponent<StageBlockController>().copyLevel++;
                    copyObject.GetComponent<StageBlockController>().Initialize(this, obj.transform.position, view, maxBlockID);
                    allStageBlocks.Add(copyObject);
                }
                //selectedObjectsClear();
                catchMode = true;
                moveVector = -1;
                yield break;
            }
            Debug.Log($"Axis = {(Input.GetAxis("Vertical") > 0)} | Count = {selectedObjects.Count > 0} | Copy = {minCopylevel} " );
            if (Input.GetAxis("Vertical") > 0 && selectedObjects.Count > 0 && minCopylevel != 0)
            {
                Debug.Log("hoge");
                foreach (var obj in selectedObjects)
                {
                    obj.transform.SetParent(this.transform);
                    obj.GetComponent<StageBlockController>().Initialize(this, obj.transform.position, view, maxBlockID);
                    allStageBlocks.Remove(obj);
                }
                //selectedObjectsClear();
                catchMode = true;
                moveVector = 1;
                yield break;
            }
        }
    }

    public void CatchModeFalse()
    {
        rb.isKinematic = false;
        catchMode = false;
        catchBlockID = -1;
        moveVector = 0;
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
