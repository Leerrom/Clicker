using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UpgradeUI : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textPrice;
    Upgrade _upgrade;
    public bool isPerm;
    int _actualCost;

    public Image button;

    public void Initialize(Upgrade upgrade)
    {
        _upgrade = upgrade;
        image.sprite = upgrade.sprite;
        textName.text = upgrade.name;
        textDescription.text = upgrade.description;
        textPrice.text = upgrade.cost + "$";
        isPerm = upgrade.ispermanent;
        _actualCost = upgrade.cost;
    }

    public void OnClick()
    {
        if (_actualCost <= MainGame.Instance.goldmoney) //Assez d'argent
        {
            MainGame.Instance.goldmoney -= _actualCost;
            MainGame.Instance.UpdateGold(MainGame.Instance.goldmoney);
            MainGame.Instance.AddUpgrade(_upgrade);
            if (!isPerm)
            {
                MainGame.Instance.DeleteNonPermanent(_upgrade);
            }
        }
        else //Pas assez d'argent
        {
            Debug.Log("Vous n'avez pas assez d'argent !");

            //Feedback
            GameObject go = GameObject.Instantiate(MainGame.Instance.prefabNotEnoughGold, MainGame.Instance.importantcanvas.transform, false);
            GameObject.Destroy(go, 1);
            if (go != null)
            {
                go.transform.DOLocalMoveY(2, 0.8f);
                go.GetComponent<TextMeshProUGUI>().DOFade(0, 0.8f);
            }
        }
    }
}
