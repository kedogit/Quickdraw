using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject m_settingsMenu;
    [SerializeField] GameObject m_mainMenu;

    public void PlayGame()
    {
        SceneManager.LoadScene("PrototypeLevel");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SettingsMenu()
    {
        m_settingsMenu.SetActive(true);
        m_mainMenu.SetActive(false);
    }

    public void BackToMain()
    {
        m_settingsMenu.SetActive(false);
        m_mainMenu.SetActive(true);
    }
}
