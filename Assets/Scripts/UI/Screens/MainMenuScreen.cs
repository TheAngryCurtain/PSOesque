using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using Rewired;

[System.Serializable]
public class UIMenuItemInfo
{
    public string m_LabelText;
    public string m_Description; // this should be moved from here to a new MenuItemDescriptionInfo created
}

public class MainMenuScreen : UIBaseScreen
{
    [SerializeField] private UIMenu m_Menu;

    public override void Initialize()
    {
        base.Initialize();

        m_Menu.OnItemSelected += OnItemSelected;
        m_Menu.PopulateMenu();
    }

    public override void Shutdown()
    {
        m_Menu.OnItemSelected -= OnItemSelected;

        base.Shutdown();
    }

    protected override void OnInputUpdate(InputActionEventData data)
    {
        base.OnInputUpdate(data);
        m_Menu.HandleInput(data);
    }

    public void OnItemSelected(int index)
    {
        switch (index)
        {
            //case 0: // Play
            //    UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Modes);
            //    break;

            //case 1: // Settings
            //    UIManager.Instance.TransitionToScreen(UI.Enums.ScreenId.Settings);
            //    break;

            case 2: // Quit
                Quit();
                break;
        }
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
