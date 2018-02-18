using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Foundation;
using System;

public abstract class MenuBase :MonoBehaviour
{
    public abstract void OnBackPressed();
    public bool _destroyWhenClosed = true;
    public bool _disableMenuUnderneath = true;
}

public abstract class Menu<T> : MenuBase where T : Menu<T>
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    public override void OnBackPressed()
    {
        Close();
    }

    protected static void Open()
    {
        if (Instance == null)
        {
            MenuManager.Instance.CreateInstance<T>();
        }
        else
        {
            Instance.gameObject.SetActive(true);
        }

        MenuManager.Instance.OpenMenu(Instance);
    }

    protected static void Close()
    {
        if (Instance == null)
        {
            return;
        }
        MenuManager.Instance.CloseMenu(Instance);
    }
}
