using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageGenerator : MonoBehaviour
{

    public GameObject stage1;
    public GameObject stage2;
    public GameObject stage3;
    public GameObject stage4;
    public GameObject stage5;
    public GameObject stage6;
    public GameObject stage7;
    public GameObject stage8;
    public int stage = 1;
    // Start is called before the first frame update
    void Start()
    {
        stage = 0;
        stage1.SetActive(true);
        stage2.SetActive(false);
        stage3.SetActive(false);
        stage4.SetActive(false);
        stage5.SetActive(false);
        stage6.SetActive(false);
        stage7.SetActive(false);
        stage8.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (stage)
        {
            case 1:
                resetStage();
                stage1.SetActive(true);
                break;
            case 2:
                resetStage();
                stage2.SetActive(true);
                break;
            case 3:
                resetStage();
                stage3.SetActive(true);
                break;
            case 4:
                resetStage();
                stage4.SetActive(true);
                break;
            case 5:
                resetStage();
                stage5.SetActive(true);
                break;
            case 6:
                resetStage();
                stage6.SetActive(true);
                break;
            case 7:
                resetStage();
                stage7.SetActive(true);
                break;
            case 8:
                resetStage();
                stage8.SetActive(true);
                break;
        }
    }

    public void Loadmenu()
    {
        Debug.Log("load scene menu");
        SceneManager.LoadScene("menu");
    }

    private void resetStage()
    {
        stage1.SetActive(false);
        stage2.SetActive(false);
        stage3.SetActive(false);
        stage4.SetActive(false);
        stage5.SetActive(false);
        stage6.SetActive(false);
        stage7.SetActive(false);
        stage8.SetActive(false);
    }
    public void Stage1()
    {
        SceneManager.LoadScene("stage1");
    }
    public void Stage2()
    {
        SceneManager.LoadScene("stage2");
    }
    public void Stage3()
    {
        SceneManager.LoadScene("stage3");
    }
    public void Stage4()
    {
        SceneManager.LoadScene("stage4");
    }public void Stage5()
    {
        SceneManager.LoadScene("stage5");
    }public void Stage6()
    {
        SceneManager.LoadScene("stage6");
    }public void Stage7()
    {
        SceneManager.LoadScene("stage7");
    }public void Stage8()
    {
        SceneManager.LoadScene("stage8");
    }

    public void select1()
    {
        stage = 1;
    }

    public void select2()
    {
        stage = 2;
    }public void select3()
    {
        stage = 3;
    }public void select4()
    {
        stage = 4;
    }public void select5()
    {
        stage = 5;
    }public void select6()
    {
        stage = 6;
    }public void select7()
    {
        stage = 7;
    }public void select8()
    {
        stage = 8;
    }
}
