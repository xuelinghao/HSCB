using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDeduce : MonoBehaviour
{
    public int num;
    public SceneState1 sceneState1;
    public SceneState1 resetState1;

    public float timer;
    public bool isStop;
    public float times;
    public float btn_times;
    public bool isOnClick = false;
    public Button next_btn;
    private void Start()
    {
        isStop = false;
    }
    // Start is called before the first frame update
    public void Init()
    {
        transform.Find("name").GetComponent<Text>().text = sceneState1.scene;
        next_btn.onClick.AddListener(() => {
            Dispose.Instance.OneJudge(gameObject, sceneState1);
            isOnClick = true;
        });
        transform.Find("Reset").GetComponent<Button>().onClick.AddListener(() => {
            if (num!=0)
            {
                isStop = false;
                isOnClick = true;
                next_btn.enabled = true;
                next_btn.GetComponent<Image>().color = new Color(0.3f, 1, 0.3f);
                transform.Find("Reset").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                next_btn.GetComponent<Item>().name = sceneState1.phases[0].introduce;

                Dispose.Instance.Reset(gameObject, resetState1);
                num = 0;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (isStop)
        {
            times += Time.deltaTime;
            next_btn.GetComponent<Button>().enabled = false;
            next_btn.GetComponent<Image>(). color= new Color(0.5f, 0.5f, 0.5f);
            if (times >= timer)
            {
                times = 0;
                isStop = false;
                isOnClick = false;
                Dispose.Instance.OneJudge(gameObject, sceneState1);
                next_btn.GetComponent<Button>().enabled = true;
                next_btn.GetComponent<Image>().color = new Color(0.3f, 1, 0.3f);
            }
        }
        else if (isOnClick)
        {
            next_btn.GetComponent<Button>().enabled = false;
            next_btn.GetComponent<Image>(). color= new Color(0.5f, 0.5f, 0.5f);
            btn_times += Time.deltaTime;
            if (btn_times > 5)
            {
                btn_times = 0;
                next_btn.GetComponent<Button>().enabled = true;
                next_btn.GetComponent<Image>(). color= new Color(0.3f, 1f, 0.3f);
                isOnClick = false;
            }
        }
        if (num == sceneState1.phases.Count) 
        {
            next_btn.GetComponent<Button>().enabled = false;
            next_btn.GetComponent<Image>(). color= new Color(0.5f,0.5f,0.5f);

        }
        if (num!=0)
        {
            transform.Find("Reset").GetComponent<Image>().color = new Color(0.3f, 1f, 0.3f);
        }
    }

}
