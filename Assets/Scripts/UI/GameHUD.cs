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
    [SerializeField] private AnimationClip m_fadeOutScreen;
    [SerializeField] private GameObject m_endScreen;

    private Animation m_blackScreenAnimator;

    void Start()
    {
        m_blackScreenAnimator = m_blackScreen.GetComponent<Animation>();
        m_blackScreenAnimator.Play();

        Observer.GetInstance().SubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
        Observer.GetInstance().SubscribeTo(EVENT.ON_LEVEL_COMPLETE, OnLevelComplete);
    }

    private void OnDestroy()
    {
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_ENEMY_HURT, ShowHitmarker);
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_LEVEL_COMPLETE, OnLevelComplete);
    }

    private void OnLevelComplete()
    {
        StartCoroutine(ShowEndScreen());
    }

    private IEnumerator ShowEndScreen()
    {
        m_blackScreenAnimator.clip = m_fadeOutScreen;
        m_blackScreenAnimator.Play();
        yield return new WaitForSeconds(1f);
        m_endScreen.SetActive(true);
        Animation textAnimator = m_endScreen.transform.Find("EndText").gameObject.GetComponent<Animation>();
        textAnimator.Play();
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
        Observer.GetInstance().SetGameState(GAME_STATE.NEWGAME);
        SceneManager.LoadScene("PrototypeLevel");
    }

    public void BackToMainMenu()
    {
        ToggleEscMenu();
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
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
