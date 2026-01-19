using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CurrentItemData : MonoBehaviour
{
    public Image ItemSprite;
    public TextMeshProUGUI ItemCount;

    private void Awake()
    {
        ItemSprite.color = Color.clear;
        ItemCount.text = "";
    }
}
