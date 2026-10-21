using UnityEngine;

public class UISFX : MonoBehaviour
{
    public void OnHover()
    {
        AudioManager.GetInstance()?.PlaySFX(SFX.MENU_HOVER);
    }

    public void OnConfirm()
    {
        AudioManager.GetInstance()?.PlaySFX(SFX.MENU_CONFIRM1);
    }

    public void OnConfirm2()
    {
        AudioManager.GetInstance()?.PlaySFX(SFX.MENU_CONFIRM2);
    }
}
