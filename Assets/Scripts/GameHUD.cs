using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Animator m_hpBarAnimator;
    [SerializeField] private GameObject m_grappleText;

    public void UpdateHP(float currHP)
    {
        m_hpBarAnimator.SetFloat("HP", currHP);
    }

    public void ShowGrappleText()
    {
        m_grappleText.SetActive(true);
    }
}
