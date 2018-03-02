using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

using Foundation;

public class MenuManager : SingletonBase<MenuManager>
{
    private Stack<MenuBase> _menuStack;

    public MainMenu _mainMenuPrefab;
    public OptionMenu _optionMenuPrefab;
    public RadialMenu _radialMenuPrefab;
    public ModalDialog _modalDialogPrefab;
    public LevelCreateMenu _levelCreateMenuPrefab;
    public InGameHUD _inGameHUDPrefab;

    private void Start()
    {
        _menuStack = new Stack<MenuBase>();
        DontDestroyOnLoad(gameObject);
        //MainMenu.Show();
    }

    public void CreateInstance<T>() where T : MenuBase
    {
        var prefab = GetPrefab<T>();
        Instantiate(prefab, transform);
    }

    public void OpenMenu(MenuBase instance)
    {
        if (_menuStack.Count > 0)
        {
            if (instance._disableMenuUnderneath)
            {
                foreach (var menu in _menuStack)
                {
                    menu.gameObject.SetActive(false);
                    if (menu._disableMenuUnderneath)
                    {
                        break;
                    }
                }
            }
            Canvas topCanvas = instance.GetComponent<Canvas>();
            Canvas prevCanvas = _menuStack.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = prevCanvas.sortingOrder + 1;
        }

        _menuStack.Push(instance);
    }

    public void CloseMenu(MenuBase menu)
    {
        if (_menuStack.Count == 0)
        {
            Debug.Log("MenuStack is empty");
            return;
        }

        if (_menuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
        }

        CloseTopMenu();
    }

    public void CloseTopMenu()
    {
        MenuBase instance = _menuStack.Pop();
        if (instance._destroyWhenClosed)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance.gameObject.SetActive(false);
        }

        foreach (var menu in _menuStack)
        {
            menu.gameObject.SetActive(true);
            if (menu._disableMenuUnderneath)
            {
                break;
            }
        }
    }

    private T GetPrefab<T>() where T : MenuBase
    {
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            var prefab = field.GetValue(this) as T;
            if (prefab != null)
            {
                return prefab;
            }
        }
        throw new MissingReferenceException("Prefab not found for type " + typeof(T));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && _menuStack.Count > 0)
        {
            _menuStack.Peek().OnBackPressed();
        }
    }
}
