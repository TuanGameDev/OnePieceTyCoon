using _Game.Scripts.Scriptable_Objects;
using _Game.Scripts.Scriptable_Objects.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class ItemObject : MonoBehaviour
{
    public ItemSO ItemData;

    public SpriteRenderer iconRenderer;

    public TextMeshProUGUI AmountTxt;

    private int _amount;

    public void Start()
    {
        StartCoroutine(DestroyAfterDelay(3f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyObject();
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    public void SetAmount(int newAmount)
    {
        _amount = newAmount;
        AmountTxt.text = _amount.ToString();
    }
    public void LoadItem()
    {
        if (ItemData != null)
        {
            iconRenderer.sprite = ItemData.ItemIcon;
        }
    }
}
