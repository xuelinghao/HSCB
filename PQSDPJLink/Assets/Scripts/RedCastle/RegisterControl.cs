using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterControl : MonoBehaviour
{
    public static RegisterControl Instance;
    public GameObject register;
    public Button register_btn;
    public InputField user;
    public InputField password;
    public Text user_hint;
    public Text password_hint;
    public GameObject dialog;
    public List<User> users;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        register_btn.onClick.AddListener(RegisterOnClick);
        user_hint.text = "";
        password_hint.text = "";
        dialog.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter)|| Input.GetKeyDown(KeyCode.Return))
        {
            RegisterOnClick();
        }
    }
    public void RegisterOnClick()
    {
        if (user.text != "" && password.text != "") 
        {
            bool isUser = false;
            bool isPassword = false;
            for (int i = 0; i < users.Count; i++)
            {
                if (user.text==users[i].user)
                {
                    isUser = true;
                    if (password.text== users[i].password)
                    {
                        isPassword = true;
                    }
                }
            }
            if (isUser&&isPassword)
            {
                Debug.Log("正确");
                dialog.SetActive(true);
            }
            else if (isUser && !isPassword)
            {
                password_hint.gameObject.SetActive(true);
                password_hint.text = "密码不正确！";
            }
            else if (!isUser)
            {
                user_hint.gameObject.SetActive(true);
                user_hint.text = "账号不存在！";
            }
        }
        else if (user.text == "")
        {
            user_hint.gameObject.SetActive(true);
            user_hint.text = "请输入账号！";
        }
        else if (password.text == "") 
        {
            password_hint.gameObject.SetActive(true);
            password_hint.text = "请输入密码！";
        }
    }
    public void Open()
    {
        dialog.SetActive(false);
        register.SetActive(false);
        RedCastleControl.Instance.OpenProject();

    }
    public void No()
    {
       register.SetActive(false);
       RedCastleControl.Instance.progress.SetActive(false);

       RedCastleControl.Instance.main.transform.SetSiblingIndex(6);
        RedCastleControl.Instance.return_obj.transform.SetAsLastSibling();

    }
}
