using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldControl: MonoBehaviour, ISelectHandler, IDeselectHandler
{
    EventSystem system;
    private bool _isSelect = false;
    public Text hint;
    public void OnDeselect(BaseEventData eventData)
    {
        _isSelect = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        _isSelect = true;
        hint.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && _isSelect)
        {
            Selectable next = null;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

                if (next == null) next = system.lastSelectedGameObject.GetComponent<Selectable>();
            }
            else
            {
                next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                if (next == null) next = system.firstSelectedGameObject.GetComponent<Selectable>();
            }
            if (next != null)
            {
                InputField inputfield = next.GetComponent<InputField>();
                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
            }
            else
            {
                Debug.LogError("找不到下一个控件");
            }
        }
    }
}
