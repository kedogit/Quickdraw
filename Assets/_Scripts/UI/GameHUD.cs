using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private InputActionAsset m_actions;
    [SerializeField] private GameObject m_blackScreen;
    [SerializeField] private Animator m_hpBarAnimator;
    [SerializeField] private GameObject m_grappleText;
    [SerializeField] private GameObject m_interactPrompt;
    [SerializeField] private GameObject m_escMenu;
    [SerializeField] private GameObject m_settingsMenu;
    [SerializeField] private GameObject m_topLevelMenu;

    [SerializeField] private AnimationClip m_fadeOutScreen;
    [SerializeField] private GameObject m_endScreen;


    private InputAction m_escMenuInput;

    private Animation m_blackScreenAnimator;
    private const float m_grappleTextHideDelay = 2f;
    private const float m_grappleTextFadeDuration = 0.5f;

    void Start()
    {
        m_blackScreenAnimator = m_blackScreen.GetComponent<Animation>();
        m_blackScreenAnimator.Play();

        m_escMenuInput = m_actions.FindAction("ESCMenu");

        Observer.GetInstance().SubscribeTo(EVENT.ON_LEVEL_COMPLETE, OnLevelComplete);
    }

    private void Update()
    {
        OpenEscMenu();
    }

    private void OpenEscMenu()
    {
        if (m_escMenuInput.WasPressedThisFrame())
        {
            if (!m_escMenu.activeSelf)
            {
                m_escMenu.SetActive(true);
                Time.timeScale = 0;
                TriggerMenuToggleEvent(true);
            }
        }
    }

    private void OnDestroy()
    {
        Observer.GetInstance().UnsubscribeTo(EVENT.ON_LEVEL_COMPLETE, OnLevelComplete);
    }

    private void OnLevelComplete()
    {
        StartCoroutine(ShowEndScreen());
    }

    private IEnumerator ShowEndScreen()
    {
        //start the fade out
        m_blackScreenAnimator.clip = m_fadeOutScreen;
        m_blackScreenAnimator.Play();

        //show end screen
        yield return new WaitForSeconds(1f);
        m_endScreen.SetActive(true);

        //fade text in
        Animation textAnimator = m_endScreen.transform.Find("EndText").gameObject.GetComponent<Animation>();
        textAnimator.Play();
    }

    public void UpdateHP(float currHP)
    {
        m_hpBarAnimator.SetFloat("Fill", currHP);
    }

    public void ToggleInteractPrompt()
    {
        m_interactPrompt.SetActive(!m_interactPrompt.activeSelf);
    }

    public void ShowGrappleText()
    {
        m_grappleText.SetActive(true);
        StartCoroutine(HideTextAfterDelay(m_grappleTextHideDelay, m_grappleTextFadeDuration, m_grappleText));
    }

    private IEnumerator HideTextAfterDelay(float delay, float fadeDuration, GameObject objectToHide)
    {
        //wait for the specified delay time
        yield return new WaitForSeconds(delay);

        //set vars for the fade
        TextMeshProUGUI text = objectToHide.GetComponent<TextMeshProUGUI>();
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            text.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        //set object inactive after fade completed
        objectToHide.SetActive(false);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        m_escMenu.SetActive(false);
        TriggerMenuToggleEvent(false);
    }

    private void TriggerMenuToggleEvent(bool menuState)
    {
        Dictionary<string, object> eventParams = new Dictionary<string, object>();
        eventParams.Add("MenuActive", menuState);
        Observer.GetInstance().TriggerEvent(EVENT.ON_MENU_TOGGLED, eventParams);
    }

    public void RestartLevel()
    {
        ResumeGame();
        Observer.GetInstance().SetGameState(GAME_STATE.NEWGAME);
        SceneManager.LoadScene("PrototypeLevel");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        TriggerMenuToggleEvent(true);
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
}
