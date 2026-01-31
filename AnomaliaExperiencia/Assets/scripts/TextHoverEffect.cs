using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TextHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Text Reference")]
    public TextMeshProUGUI text;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.red;

    private Quaternion originalRotation;

    void Start()
    {
        if (text != null)
        {
            originalRotation = text.rectTransform.rotation;
            text.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text == null) return;

        text.color = hoverColor;
        text.rectTransform.rotation = Quaternion.Euler(180f, 0f, 0f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (text == null) return;

        text.color = normalColor;
        text.rectTransform.rotation = originalRotation;
    }
}