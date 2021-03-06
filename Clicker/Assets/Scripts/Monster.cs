using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Monster : MonoBehaviour
{
    public int maxHP;
    [SerializeField]int HP; //vie actuelle du monste
    public GameObject visualLVL;
    public GameObject textlife; //texte de vie
    public Image healthbar; //visuel barre de vie
    public GameObject visual; //visuel du monstre
    public int monsterReward; //gold du monstre
    public int monsterLVL = 0;

    void Start()
    {
        HP = maxHP;
        UpdateHPvisuals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateHPvisuals() //actualise le texte de vie et la barre de vie quand un clic est fait
    {
        //Update du texte
        textlife.GetComponent<UnityEngine.UI.Text>().text = HP + " / " + maxHP;
        visualLVL.GetComponent<UnityEngine.UI.Text>().text = "LEVEL " + monsterLVL;
        //Update de la barre de vie
        float percent = (float)HP / (float)maxHP;
        healthbar.fillAmount = percent;
    }

    public void Hit(int damage) //Le monstre prend un coup
    {
        HP -= damage;
        UpdateHPvisuals();

        //Effet punch
        visual.transform.DOComplete(); //termine le tween en cours
        visual.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f);
    }

    public void SetMonster(MonsterInfos index)
    {
        monsterLVL++;
        maxHP = index.HP;
        HP = maxHP;
        UpdateHPvisuals();
        visual.GetComponent<SpriteRenderer>().sprite = index.sprite;
        monsterReward = index.reward;
    }

    public bool isAlive()
    {
        return HP > 0;
    }
}
