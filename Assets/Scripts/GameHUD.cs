using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] private Animator m_hpBarAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateHP(float currHP)
    {
        m_hpBarAnimator.SetFloat("HP", currHP);
    }
}
