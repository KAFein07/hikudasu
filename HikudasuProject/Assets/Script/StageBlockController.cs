using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBlockController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Material[] _materials;

    public int copyLevel = 0;
    public int blockID;

    private float moveValue = 1f;
    private int _view;
    private int maxID;
    private Vector3 _objPos;
    private Vector3 pos;
    private PlayerController _playerController;
    private MeshRenderer _meshRenderer;
    private BoxCollider _boxCollider;

    private void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
        switch (blockID)
        {
            case 1:
                _materials = _playerController.yellowMaterials;
                break;
            case 2:
                _materials = _playerController.redMaterials;
                break;
            case 3:
                _materials = _playerController.greenMaterials;
                break;
            case 4:
                _materials = _playerController.pinkMaterials;
                break;
        }
    }

    private void OnEnable()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void Initialize(PlayerController playerController, Vector3 objPos, int view, int max)
    {
        //maxID = max;
        Debug.Log("maxID = " + maxID);
        _playerController = playerController;
        _objPos = objPos;
        _view = view;
        switch (blockID % max)
        {
            case 1:
                _materials = _playerController.yellowMaterials;
                break;
            case 2:
                _materials = _playerController.redMaterials;
                break;
            case 3:
                _materials = _playerController.greenMaterials;
                break;
            case 0:
                _materials = _playerController.pinkMaterials;
                break;
        }
        ChangeColorHigh();
        StartCoroutine(BlockCroutine());
    }

    private IEnumerator BlockCroutine()
    {
        _boxCollider.enabled = false;
        while (true)
        {
            pos = transform.position;
            switch (_view)
            {
                case 1:
                    if (pos.z <= _objPos.z - moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        Debug.Log(_objPos);
                        pos.z = _objPos.z - moveValue;
                        pos.x = _objPos.x;
                        BlockMoveFinish(true);
                        yield break;
                    }
                    else if (pos.z >= _objPos.z + moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        BlockMoveFinish(false);
                        yield break;
                    }
                    break;
                case 2:
                    if (pos.x <= _objPos.x - moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        Debug.Log(_objPos);
                        pos.x = _objPos.x - moveValue;
                        pos.z = _objPos.z;
                        BlockMoveFinish(true);
                        yield break;
                    }
                    else if (pos.x >= _objPos.x + moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        BlockMoveFinish(false);
                        yield break;
                    }
                    break;
                case 3:
                    if (pos.z >= _objPos.z + moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        Debug.Log(_objPos);
                        pos.z = _objPos.z + moveValue;
                        pos.x = _objPos.x;
                        BlockMoveFinish(true);
                        yield break;
                    }
                    else if (pos.z <= _objPos.z - moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        BlockMoveFinish(false);
                        yield break;
                    }
                    break;
                case 4:
                    if (pos.x >= _objPos.x + moveValue)
                    {
                        _player.GetComponent<PlayerController>().rbFin();
                        Debug.Log(_objPos);
                        pos.x = _objPos.x + moveValue;
                        pos.z = _objPos.z;
                        BlockMoveFinish(true);
                        yield break;
                    }
                    else if (pos.x <= _objPos.x - moveValue)
                    {
                        Debug.Log("owari");
                        _player.GetComponent<PlayerController>().rbFin();
                        BlockMoveFinish(false);
                        yield break;
                    }
                    break;
            }
            yield return null;
        }
    }
    private void BlockMoveFinish(bool _bool)
    {
        if (_bool)
        {
            transform.SetParent(null, true);
            pos.y = _objPos.y;
            transform.position = pos;
            _boxCollider.enabled = true;
            //blockID += maxID;
            //Debug.Log("blockID = " + blockID);
        }
        else
            Destroy(this.gameObject);
        _playerController.CatchModeFalse();
    }

    public void ChangeColorLow()
    {
        Color _color = _meshRenderer.material.color;
        float fadeAmount = 0.5f;
        Color.RGBToHSV(_color, out float h, out float s, out float v);
        s *= fadeAmount;
        Color fadedColor = Color.HSVToRGB(h, s, v);
        _meshRenderer.material.color = fadedColor;
    }
    public void ChangeColorHigh()
    {
        _meshRenderer.material = _materials[copyLevel];
    }
}