using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageGenerator : MonoBehaviour
{

    public GameObject stage1;
    public GameObject stage2;
    // Start is called before the first frame update
    void Start()
    {
        stage1.SetActive(true);
        stage2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Loadmenu()
    {
        Debug.Log("load scene menu");
        SceneManager.LoadScene("menu");
    }

    public void Stage1()
    {
        SceneManager.LoadScene("stage1");
    }

    public void Stage2()
    {
        SceneManager.LoadScene("stage2");
    }

    public void select1()
    {
        stage1.SetActive(true);
        stage2.SetActive(false);
    }

    public void select2()
    {
        stage1.SetActive(false);
        stage2.SetActive(true);
    }
}
