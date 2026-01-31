using UnityEngine;

public class MenuPanels : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject controlsMenu;

    public void ShowControls()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void ShowMainMenu()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}