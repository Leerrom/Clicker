using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textPrice;

    Upgrade _upgrade;

    public void Initialize(Upgrade upgrade)
    {
        _upgrade = upgrade;
        image.sprite = upgrade.sprite;
        textDescription.text = upgrade.name + System.Environment.NewLine + upgrade.description;
        textPrice.text = upgrade.cost + "$";
    }
}
