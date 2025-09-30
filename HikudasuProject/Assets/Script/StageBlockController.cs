using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageBlockController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Material defaltBlockMaterial;
    [SerializeField] private Material catchBlockMaterial;
    //[SerializeField] private Material yellow;
    //[SerializeField] private Material red;
    //[SerializeField] private Material blue;
    //[SerializeField] private Material green;

    public int blockID;

    //private int copyLevel = 0;
    private bool catchObject;
    private int _view;
    private Camera camera;

    //[SerializeField] private float _blockPosX;
    //[SerializeField] private float _blockPosY;
    //[SerializeField] private float _blockPosZ;

    PlayerController _playerController;
    //Vector3 pos;
    //Vector3 nullPos = Vector3.zero;
    MeshRenderer mr;
    Ray ray;
    RaycastHit hit;

    //GameObject[] objs;
    //Dictionary<float, List<GameObject>> groupsX = new Dictionary<float, List<GameObject>>();
    //Dictionary<float, List<GameObject>> groupsZ = new Dictionary<float, List<GameObject>>();

    // Start is called before the first frame update
    void OnEnable()
    {
        /*
        if (mr.material == yellow)
        {
            mr.material = red;
        }
        else if (mr.material == red)
        {
            mr.material = blue;
        }
        else if (mr.material == blue)
        {
            mr.material = green;
        }
        else if(mr.material == green)
        {
            Debug.Log("Bag");
        }
        else
        {
            mr.material = yellow;
        }//*/
        _playerController = _player.GetComponent<PlayerController>();
        mr = GetComponent<MeshRenderer>();
        camera = Camera.main;
        //pos = transform.position;
        /*
        objs = GameObject.FindGameObjectsWithTag("Stage");
        foreach (GameObject obj in objs)
        {
            float x = obj.transform.position.x;

            if (!groupsX.ContainsKey(x))
            {
                groupsX[x] = new List<GameObject>();
            }
            groupsX[x].Add(obj);
        }
        foreach (GameObject obj in objs)
        {
            float z = obj.transform.position.z;

            if (!groupsZ.ContainsKey(z))
            {
                groupsZ[z] = new List<GameObject>();
            }
            groupsZ[z].Add(obj);
        }//*/
    }

    // Update is called once per frame
    void Update()
    {
        //_blockPosX = _playerController.blockPosX;
        //_blockPosY = _playerController.blockPosY;
        //_blockPosZ = _playerController.blockPosZ;

        _view = _playerController.view;
        //ray = camera.ScreenPointToRay(Input.mousePosition);

        if (_playerController.canCatch && Input.GetMouseButtonDown(0))
        {
            _playerController.ObjectSelect(this.blockID);
        }

        /*
        if ((_view == 1 || _view == 3) && (_blockPosY == pos.y && _blockPosZ == pos.z))
        {
            mr.material = catchBlockMaterial;
        }
        else if ((_view == 2 || _view == 4) && (_blockPosY == pos.y && _blockPosX == pos.x))
        {
            mr.material = catchBlockMaterial;
        }
        else
        {
            mr.material = defaltBlockMaterial;
        }//*/
        /*
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Stage"))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    SetHighlight(this.gameObject, true);

                    if (Input.GetMouseButtonDown(0))
                    {
                        catchObject = true;
                        if (_playerController.selectedObjects.Contains(this.gameObject))
                        {
                            _playerController.selectedObjects.Remove(this.gameObject);
                            SetHighlight(this.gameObject, false);
                        }
                        else
                        {
                            _playerController.selectedObjects.Add(this.gameObject);
                            SetHighlight(this.gameObject, true);
                        }
                    }
                }//*
                else
                {
                    if (!_playerController.selectedObjects.Contains(this.gameObject))
                    {
                        SetHighlight(this.gameObject, false);
                    }
                }//*
                //*
                if (hit.collider.CompareTag("Stage"))
                {
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        mr.material = catchBlockMaterial;
                        _playerController.PositionCheck(pos);
                    }
                    else
                    {
                        mr.material = defaltBlockMaterial;
                    }
                }
                else
                {
                    _playerController.PositionNull();
                }//*
            }
            else
            {
                ClearSelection();
            }
        }

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);//*/
    }

    /*
    public void ColorChange()
    {
        Debug.Log("colorChange");
        switch (copyLevel)
        {
            case 0:
                //Red
                mr.material = red;
                break;
            case 1:
                //Blue
                mr.material = blue;
                break;
            case 2:
                //Grean
                mr.material = green;
                break;
        }
        copyLevel++;
    }//*/

    private void ClearSelection()
    {
        foreach (var obj in _playerController.selectedObjects)
        {
            SetHighlight(obj, false);
        }
        _playerController.selectedObjects.Clear();
    }

    private void SetHighlight(GameObject obj, bool selected)
    {
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = selected ? catchBlockMaterial : defaltBlockMaterial;
        }
    }

    /*
    private void hoge(GameObject reference, Material marterial)
    {
        float refZ = reference.transform.position.z;

        foreach (GameObject obj in objs)
        {
            if (Mathf.Abs(obj.transform.position.z - refZ) == 0)
            {
                groupsX.Add(obj);
            }
        }

        foreach (GameObject obj in groupsX)
        {
            obj.GetComponent<MeshRenderer>().material = material;
        }
    }//*/

}
