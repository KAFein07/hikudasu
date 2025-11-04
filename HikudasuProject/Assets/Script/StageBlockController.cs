using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class StageBlockController : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Material[] _materials;
    PlayerController _playerController;
    MeshRenderer _meshRenderer;
    public int copyLevel = 0;
    public int blockID;
    private Vector3 _objPos;
    private int _view;
    private Vector3 pos;

    float moveValue = 1f;
    bool end = false;
    float diff;
    int maxID;

    private void Awake()
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
        }
    }

    private void OnEnable()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Initialize(PlayerController playerController, Vector3 objPos, int view, int max)
    {
        maxID = max;
        _playerController = playerController;
        _objPos = objPos;
        _view = view;
        switch (blockID % max)
        {
            case 1:
                _materials = _playerController.yellowMaterials;
                
                break;
            case 0:
                _materials = _playerController.redMaterials;
                break;
        }
        ChangeColorHigh();
        StartCoroutine(BlockCroutine());
    }

    private IEnumerator BlockCroutine()
    {
        while (true)
        {
            pos = transform.position;
            switch (_view)
            {
                case 1:
                    if (pos.z <= _objPos.z - moveValue)
                    {
                        pos.z = _objPos.z - moveValue;
                        transform.SetParent(null, true);
                        transform.position = pos;
                        copyLevel++;
                        blockID += maxID;
                        ChangeColorHigh();
                        yield break;
                    }
                    else if (pos.z >= _objPos.z + moveValue)
                    {
                        pos.z = _objPos.z + moveValue;
                        transform.SetParent(null, true);
                        transform.position = pos;
                        ChangeColorHigh();
                        yield break;
                    }
                    break;
                case 2:
                    if (pos.x <= _objPos.x - moveValue)
                    {
                        pos.x = _objPos.x - moveValue;
                        transform.SetParent(null, true);
                        transform.position = pos;
                        ChangeColorHigh();
                        yield break;
                    }
                    else if (pos.x >= _objPos.x + moveValue)
                    {
                        pos.x = _objPos.x + moveValue;
                        transform.SetParent(null, true);
                        transform.position = pos;
                        ChangeColorHigh();
                        yield break;
                    }
                    break;
            }
            if (_view == 1 || _view == 3)
            {
                if (pos.z <= _objPos.z - moveValue)
                {
                    pos.z = _objPos.z - moveValue;
                    transform.SetParent(null, true);
                    transform.position = pos;
                    copyLevel++;
                    blockID += maxID;
                    ChangeColorHigh();
                    yield break;
                }
                else if (pos.z >= _objPos.z + moveValue)
                {
                    pos.z = _objPos.z + moveValue;
                    transform.SetParent(null, true);
                    transform.position = pos;
                    ChangeColorHigh();
                    yield break;
                }
            }
            else
            {
                if (pos.x <= _objPos.x - moveValue)
                {
                    pos.x = _objPos.x - moveValue;
                    transform.SetParent(null, true);
                    transform.position = pos;
                    ChangeColorHigh();
                    yield break;
                }
                else if (pos.x >= _objPos.x + moveValue)
                {
                    pos.x = _objPos.x + moveValue;
                    transform.SetParent(null, true);
                    transform.position = pos;
                    ChangeColorHigh();
                    yield break;
                }
            }
            yield return null;
        }
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
