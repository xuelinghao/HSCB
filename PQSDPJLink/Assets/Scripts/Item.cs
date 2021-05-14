using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    public string name;
    //当鼠标进入UI
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.GetComponent<Image>().color != new Color(0.5f, 0.5f, 0.5f,1))
        {
            transform.Find("Item").GetComponent<Text>().text = name;
            transform.Find("Item").gameObject.SetActive(true);
        }
    }

    //当鼠标离开UI
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.Find("Item").gameObject.SetActive(false);

    }
    private void Start()
    {
        transform.Find("Item").gameObject.SetActive(false);
        transform.Find("Item").GetComponent<Text>().text = name;
        
    }
}
