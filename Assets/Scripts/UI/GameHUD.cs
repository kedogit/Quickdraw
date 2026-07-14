using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private GameObject m_blackScreen;
    [SerializeField] private Animator m_hpBarAnimator;
    [SerializeField] private GameObject m_grappleText;
    [SerializeField] private GameObject m_escMenu;
    [SerializeField] private GameObject m_settingsMenu;
    [SerializeField] private GameObject m_topLevelMenu;
    [SerializeField] private GameObject m_hitMarker;
    [SerializeField] private AudioSource m_hitMarkerSource;

    [SerializeField] private float m_hitmarkerDuration = 0.2f;

    void Start()
    {
        Debug.Log("start call");
        m_blackScreen.GetComponent<Animation>().Play();

        Observer.GetInstance().SubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
    }

    private void OnDestroy()
    {
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
    }


    public void UpdateHP(float currHP)
    {
        m_hpBarAnimator.SetFloat("HP", currHP);
    }

    public void ShowGrappleText()
    {
        m_grappleText.SetActive(true);
    }

    public void ToggleEscMenu()
    {
        m_escMenu.SetActive(!m_escMenu.activeSelf);
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        ToggleEscMenu();

        //yucky, do this with an observer later
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().TogglePlayerInputs();
    }

    public void RestartLevel()
    {
        ResumeGame();
        SceneManager.LoadScene("PrototypeLevel");
    }

    public void BackToMainMenu()
    {
        ToggleEscMenu();
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettingsMenu()
    {
        m_topLevelMenu.SetActive(false);
        m_settingsMenu.SetActive(true);
    }

    public void ReturnToTopLevel()
    {
        m_topLevelMenu.SetActive(true);
        m_settingsMenu.SetActive(false);
    }

    public void ShowHitmarker()
    {
        Debug.Log("show hitmarker");
        m_hitMarker.SetActive(true);
        StartCoroutine(EraseHitmarker());
        m_hitMarkerSource.Play();
    }

    private IEnumerator EraseHitmarker()
    {
        yield return new WaitForSeconds(m_hitmarkerDuration);
        m_hitMarker.SetActive(false);
    }
}
