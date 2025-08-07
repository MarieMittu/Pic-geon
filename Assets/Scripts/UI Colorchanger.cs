using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIColorchanger : MonoBehaviour
{
    public TextMeshProUGUI[] textGraphics;
    public Image[] backgroundGraphics;

    public Color textNormalColor = Color.white;
    public Color textHighlightedColor = Color.yellow;
    public Color bgNormalColor = Color.white;
    public Color bgHighlightedColor = Color.blue;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.transition = Selectable.Transition.None; // Desactivar la transicion del Button

        // Asignar eventos del Button al script
        var colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        button.colors = colors;

      

        UpdateColors(textNormalColor, bgNormalColor);
    }

    public void OnPointerEnter()
    {
        UpdateColors(textHighlightedColor, bgHighlightedColor);
    }

    public void OnPointerExit()
    {
        UpdateColors(textNormalColor, bgNormalColor);
    }

    private void UpdateColors(Color newTextColor, Color newBgColor)
    {
        foreach (var graphic in textGraphics)
        {
            if (graphic != null) graphic.color = newTextColor;
        }

        foreach (var graphic in backgroundGraphics)
        {
            if (graphic != null) graphic.color = newBgColor;
        }
    }










}
