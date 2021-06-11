using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerLifeStockControl : MonoBehaviour
{
    public TextMeshProUGUI lifeStock_number_text;
    public Image panel;
    public void SetUp(Color _color)
    {
        panel.color = _color;
        lifeStock_number_text.color = _color;
    }
}
