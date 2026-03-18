using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject missionSelectPanel;
    public GameObject planeSelectPanel;
    public GameObject briefingPanel;
    public GameObject loadingPanel;

    void Start()
    {
        ShowMainMenu();
    }

    void DisableAllPanels()
    {
        mainMenuPanel.SetActive(false);
        missionSelectPanel.SetActive(false);
        planeSelectPanel.SetActive(false);
        briefingPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void ShowMainMenu()
    {
        DisableAllPanels();
        mainMenuPanel.SetActive(true);
    }

    public void ShowMissionSelect()
    {
        DisableAllPanels();
        missionSelectPanel.SetActive(true);
    }

    public void ShowPlaneSelect()
    {
        DisableAllPanels();
        planeSelectPanel.SetActive(true);
    }

    public void ShowBriefing()
    {
        DisableAllPanels();
        briefingPanel.SetActive(true);
    }

    public void StartMission()
    {
        DisableAllPanels();
        loadingPanel.SetActive(true);

        Invoke("LoadMission", 2f);
    }

    void LoadMission()
    {
        SceneManager.LoadScene("Mission_01");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}