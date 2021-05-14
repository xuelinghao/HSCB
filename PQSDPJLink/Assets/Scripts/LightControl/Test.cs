using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public InputField port;
    public InputField scene_name;
    public InputField file_name;
    public Button send_btn;
    public Button stop_btn;
    // Start is called before the first frame update
    void Start()
    {
        send_btn.onClick.AddListener(() => { StartCoroutine(Play0nClick()); });
        stop_btn.onClick.AddListener(StopOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StopOnClick()
    {
        LightControl.Instance.Send(scene_name.text, file_name.text, LigntType.Stop);

    }
    public IEnumerator Play0nClick()
    {
        CycleTypeOnClick(CycleType.stop);
        yield return new WaitForSeconds(0.1f);
            LightControl.Instance.Send(scene_name.text, file_name.text, LigntType.Play);
        
    }
    public void CycleTypeOnClick(CycleType cycleType)
    {
        switch (cycleType)
        {
            case CycleType.stop:
                LightControl.Instance.Send(scene_name.text, file_name.text, LigntType.OneStop);
                break;
            case CycleType.one:
                LightControl.Instance.Send(scene_name.text, file_name.text, LigntType.OneCycle);
                break;
        }
    }
}
