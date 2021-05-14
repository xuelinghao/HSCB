using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StepInputFiled : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    EventSystem system;
    public bool _isSelect = false;
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelect = false;
        print("没选中");
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelect = true;
        print("选中");
    }
}
