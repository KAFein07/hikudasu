using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraCenter;
    [SerializeField] private GameObject lowLight;
    [SerializeField] private GameObject resultText;

    public Material[] yellowMaterials;
    public Material[] redMaterials;
    public Material[] greenMaterials;
    public Material[] pinkMaterials;
    public Material[] whiteMaterials;
    public List<GameObject> selectedObjects = new List<GameObject>();
    public bool canCatch = false;
    public int view = 1;

    private float moveSpeed = 2;
    private float rotateSpeed = 10;
    [SerializeField] private float jumpPower = 250;
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
    private new GameObject camera;
    private List<GameObject> allStageBlocks;
    private bool isPaused = false;
    public string scene;
    public int esc = 0;
    private bool fin = false;
    private GameObject breakBlock;
    private bool canBreak = false;

    public GameObject escBtn;

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
        fin = false;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "stage3" ||
            sceneName == "stage4")
        {
            jumpPower = 350;
        }

        if (sceneName == "stage5" || 
            sceneName == "stage6")
        {
            canBreak = true;
        }
        breakBlock = null;
    }

    private void Update()
    {
        if(canCatch == false)
            rb.useGravity = true;
        if (fin) Application.Quit();
        if (Input.GetKey(KeyCode.Escape))
        {
            esc = 1;
            StPause();
        }

        if (isPaused)
        {
            StPause();
            return;
        }
        
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
        else
            moveSpeed = 2f;

        // 移動処理
        if (canMove)
        {
            moveDirection = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, verticalInput * moveSpeed * Time.deltaTime);
            transform.position += moveDirection;
        }
            //if(camera.transform.position.y > 6 && camera.transform.position.y < 12)
                camera.transform.position += scrollInput * camera.transform.forward * zoomSpeed * Time.deltaTime;


        // ジャンプ処理
        if (Input.GetKeyDown(KeyCode.Space) && canJump && catchMode == false)
        {
            rb.AddForce(Vector3.up * jumpPower);
            canJump = false;
        }

        if (Input.GetMouseButtonDown(1) && canBreak)
        {
            BlockBlast();
            anim.SetTrigger("break");
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
            rb.useGravity = false;
            ObjectSelect(catchBlockID);
            catchMode = true;
            Debug.Log("Mouse Down");
        }


        if (!Input.GetMouseButton(0) && (catchMode == false))
        {
            Debug.Log("Mouse Up");
            selectedObjectsClear();
        }

        //Debug.Log(Input.GetAxis("Vertical"));
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
            Debug.Log("aaa");

            
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
                moveVector = -1;
                Debug.Log("hiku");
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
                moveVector = 1;
                //Debug.Log("osu");
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

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" )                                      // 当たった相手のタグがGroundの場合
            canJump = true;                                                             //  ジャンプ可能
        if (collision.gameObject.tag == "Stage")                                        // 当たった相手のタグがStageの場合
        {
            float posY = transform.position.y;                                          // posYに自分の高度を格納
            float blockPosY = collision.transform.position.y;                           // blockPosYに相手の高度を格納
            float diff = Mathf.Abs(blockPosY - posY);                                   // diffに相手と自分の高度の差を格納（絶対値）
            if (diff <= 0.1f)                                                           // diffが0.1以下の場合
            {
                breakBlock = collision.gameObject;                                      // 相手を破壊可能オブジェクトとして扱う
                catchBlockID = breakBlock.GetComponent<StageBlockController>().blockID; // 相手の引っ張る時のIDを取得
                canCatch = true;                                                        // 相手を引っ張り可能オブジェクトとして扱う
            }
            List<ContactPoint> contactPoints = new();                                   // コライダー同士の接点を格納するリストを作成
            collision.GetContacts(contactPoints);                                       // コライダー同士の接点をリストへ格納
            var col = contactPoints                                                     // Linqを使用して、コライダー同士の接点のリストから
                .Where(x => x.normal.y > 0.5);                                          //  接点のY座標が0.5以上（上向きは+,下向きは-)を取得
            if (col.Count() > 0)                                                        // もし、接点のY座標が0.5以上のものが存在した場合
            {
                canJump = true;                                                         // ジャンプ可能
            }
        }
        //if (collision.gameObject.tag == "Breakable")
        //{
        //    float posY = transform.position.y;                                          // posYに自分の高度を格納
        //    float blockPosY = collision.transform.position.y;                           // blockPosYに相手の高度を格納
        //    float diff = Mathf.Abs(blockPosY - posY);                                   // diffに相手と自分の高度の差を格納（絶対値）
        //    if (diff <= 0.1f)                                                           // diffが0.1以下の場合
        //    {
        //        breakBlock = collision.gameObject;                                      // 相手を破壊可能オブジェクトとして扱う
        //    }
        //}
        if (collision.gameObject.tag == "Flag")
        {
            lowLight.SetActive(true);
            resultText.SetActive(true);
            this.enabled = false;
            StartCoroutine(ClearCoroutine());
        }
    }
private void BlockBlast()
    {
        if (breakBlock != null)
        {
            breakBlock.SetActive(false);
        }

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Stage")    // Stageタグがついたコライダーから離れた場合、
        {
            breakBlock = null;
            catchBlockID = -1;
            canCatch = false;
            canJump = false;                        // ジャンプ不可（ジャンプした際のリセット）
        }
        if (collision.gameObject.tag == "Ground")   // Groundタグがついたコライダーから離れた場合
        {
            canJump = false;                        // ジャンプ不可（ジャンプした際のリセット）
        }
    }

    private IEnumerator ClearCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("menu");
                lowLight.SetActive(false);
                resultText.SetActive(false);
                this.enabled = true;
            }
            yield return null;
        }
    }

    public void StPause()
    {
        isPaused = true;
        StartCoroutine(UnPause());
    }

    IEnumerator UnPause()
    {
        if (esc == 1)
        {
            escBtn.SetActive(true);
        }
        else
        {
            escBtn.SetActive(false);
            isPaused = false;
            yield return null;
        }

    }

    public void back()
    {
        esc = 0;
    }
    public void backMenu()
    {
        SceneManager.LoadScene("menu");
    }
    public void reTry()
    {
        SceneManager.LoadScene(scene);
    }

    public void Fin()
    {
        fin = true;
    }

    public void rbFin()
    {
        Debug.Log("rb true");
        rb.useGravity = true;
    }
    /*
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
    */
}
