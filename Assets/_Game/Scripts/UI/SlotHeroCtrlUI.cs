using UnityEngine;
using UnityEngine.UI;

public class SlotHeroCtrlUI : MonoBehaviour
{
    [SerializeField]
    private Image heroImage;

    public void SetHeroCtrlUI(Sprite heroAvatar)
    {
        if (heroAvatar != null)
        {
            heroImage.sprite = heroAvatar;
        }
    }
}
