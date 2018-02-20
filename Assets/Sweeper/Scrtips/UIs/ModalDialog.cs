using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class DialogContent
{
    public string _content = "";
    public string _okText = "OK";
    public string _cancleText = "Cancle";
    public UnityAction _action;
}


public class ModalDialog : Menu<ModalDialog>
{
    public Text _content;
    public Text _okText;
    public Text _cancleText;

    public Button _okButton;
    public Button _cancleButton;

    public static void Show(DialogContent content)
    {
        Open();

        Instance._content.text = content._content;
        Instance._okText.text = content._okText;
        Instance._cancleText.text = content._cancleText;

        Instance._okButton.onClick.AddListener(content._action);
        Instance._cancleButton.onClick.AddListener(() => Shut());
    }

    public static void Shut()
    {
        Close();
    }
}
