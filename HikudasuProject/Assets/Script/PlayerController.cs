using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private GameObject lowLight;
    [SerializeField] private GameObject resultText;

    public Material[] yellowMaterials;
    public Material[] redMaterials;
    public Material[] greenMaterials;
    public Material[] pinkMaterials;
    public List<GameObject> selectedObjects = new List<GameObject>();
    public bool canCatch = false;
    public int view = 1;

    private float moveSpeed = 2;
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
    private bool stop;
    private int catchBlockID = -1;
    private int maxBlockID = 0;
    private int moveVector;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private GameObject camera;
    private List<GameObject> allStageBlocks;
    private int scene = 0;
    private int Stage = 1;
    private int StageMax = 2;
    public GameObject TitleImg;
    public GameObject stage1;
    public GameObject stage2;

    private Animator anim;


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

        Transform child = transform.Find("u");
        anim = child.GetComponent<Animator>();
        scene = 0;
        Stage = 1;
        TitleImg.SetActive(true);
        stage1.SetActive(false);
        stage2.SetActive(false);
    }

    private void Update()
    {
       
            switch (scene)
            {
                case 0://タイトル
                    TitleImg.SetActive(true);
                   break;
                case 1://ステージ選択
                if (Stage == 1)
                {
                    resetStege();
                    stage1.SetActive(true);
                }
                if(Stage == 2)
                {
                    resetStege();
                    stage2.SetActive(true);
                }
                    break;
                case 2:
                    if (canJump == false)
                        anim.SetBool("jump", true);
                    else
                        anim.SetBool("jump", false);
                    if (moveDirection.magnitude > 0)
                        anim.SetBool("walk", true);
                    else
                        anim.SetBool("walk", false);
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
                                horizontalInput = 0;
                                if (verticalInput < 0)
                                    verticalInput = 0;
                                break;
                            case 2:
                                verticalInput = 0;
                                if (horizontalInput < 0)
                                    horizontalInput = 0;
                                break;
                            case 3:
                                horizontalInput = 0;
                                if (verticalInput > 0)
                                    verticalInput = 0;
                                break;
                            case 4:
                                verticalInput = 0;
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
                                horizontalInput = 0;
                                if (verticalInput > 0)
                                    verticalInput = 0;
                                break;
                            case 2:
                                verticalInput = 0;
                                if (horizontalInput > 0)
                                    horizontalInput = 0;
                                break;
                            case 3:
                                horizontalInput = 0;
                                if (verticalInput < 0)
                                    verticalInput = 0;
                                break;
                            case 4:
                                verticalInput = 0;
                                if (horizontalInput < 0)
                                    horizontalInput = 0;
                                break;
                        }
                    }

                if (catchMode)
                    moveSpeed = 1.5f;
                else if (stop)
                    moveSpeed = 0f;
                else
                    moveSpeed = 2f;

                    // 移動処理
                    if (canMove)
                    {
                        moveDirection = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, verticalInput * moveSpeed * Time.deltaTime);
                        transform.position += moveDirection;
                    }
                    camera.transform.position += scrollInput * camera.transform.forward * zoomSpeed * Time.deltaTime;

                    // ジャンプ処理
                    if (Input.GetKeyDown(KeyCode.Space) && canJump && catchMode == false)
                    {
                        rb.AddForce(Vector3.up * jumpPower);
                        canJump = false;
                    }

                    // 滑らかな向き変更
                    if ((horizontalInput != 0 || verticalInput != 0) && (catchMode == false))
                        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

                    //視点変更
                    if (Input.GetKeyDown(KeyCode.Q) && canRotate && catchMode == false)
                    {
                        StartCoroutine(RotationCoroutine(1));
                        canRotate = false;
                    }
                    if (Input.GetKeyDown(KeyCode.E) && canRotate && catchMode == false)
                    {
                        StartCoroutine(RotationCoroutine(-1));
                        canRotate = false;
                    }

                    /*
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
                    }*/

                    if (canCatch && Input.GetMouseButtonDown(0) && (catchMode == false))
                    {
                        ObjectSelect(catchBlockID);
                    }


                    if (Input.GetMouseButtonUp(0) && (catchMode == false))
                    {
                        selectedObjectsClear();
                    }

                    //Debug.Log(Input.GetAxis("Vertical"));
                    break;

            }
    }

    private void selectedObjectsClear()
    {
        anim.SetBool("pull", false);
        selectedObjects.Clear();
        //foreach (var obj in allStageBlocks)
           // obj.GetComponent<StageBlockController>().ChangeColorHigh();
       
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
        catchMode = true;
        anim.SetBool("pull", true);

        yield return new WaitForSeconds(0.2f);
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
                //catchMode = true;
                moveVector = -1;
                //Debug.Log("hiku");
                //anim.SetBool("pull",true);
                yield break;
            }
            //Debug.Log($"Axis = {(Input.GetAxis("Vertical") > 0)} | Count = {selectedObjects.Count > 0} | Copy = {minCopylevel} " );
            if (Input.GetAxis("Vertical") > 0 && selectedObjects.Count > 0 && minCopylevel != 0)
            {
                //Debug.Log("hoge");
                foreach (var obj in selectedObjects)
                {
                    obj.transform.SetParent(this.transform);
                    obj.GetComponent<StageBlockController>().Initialize(this, obj.transform.position, view, maxBlockID);
                    allStageBlocks.Remove(obj);
                }
                //selectedObjectsClear();
                //catchMode = true;
                moveVector = 1;
                //Debug.Log("osu");
                //anim.SetBool("pull", true);
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
        anim.SetBool("pull", false);
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
            StartCoroutine(ClearCoroutine());
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

    private IEnumerator ClearCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                lowLight.SetActive(false);
                resultText.SetActive(false);
                this.enabled = true;
                foreach (var obj in allStageBlocks)
                {
                    if (obj.GetComponent<StageBlockController>().copyLevel > 0)
                    {
                        //allStageBlocks.Remove(obj);
                        Destroy(obj);                        
                    }
                }
                scene = 1;
                break;
            }
            yield return null;
        }
    }
    public void StartImg()
    {
        TitleImg.SetActive(false);
        stage1.SetActive(true);
        scene = 1;
    }

    public void changeStageup()
    {
        if(Stage < StageMax)Stage++;
    }
    public void changeStagedown()
    {
        if(Stage > 1)Stage--;
    }

    public void Startstage()
    {
        stage1.SetActive(false);
        stage2.SetActive(false);
        scene = 2;
        if (Stage == 1)
        {
            this.transform.position = new Vector3(0, 0.5f, -3);
            cameraCenter.position = new Vector3(0, 0, 0);
        }
        if (Stage == 2)
        {
            this.transform.position = new Vector3(17, 0.5f, -3);
            cameraCenter.position = new Vector3(17, 0, 0);
        }
        this.transform.rotation = Quaternion.identity;


    }
    private void resetStege()
    {
        stage1.SetActive(false);
        stage2.SetActive(false);
    }
}
