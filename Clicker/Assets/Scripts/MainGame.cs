using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainGame : MonoBehaviour
{
    //GameObject monster;
    int monsterMaxHP; //HP max
    int monsterHP; //HP actuelle du monstre
    public List<MonsterInfos> monsters;

    int _currentMonster; //index du monstre actuel
    public Monster monster;

    public Canvas canvas;
    public Canvas importantcanvas;
    public GameObject prefabDamageFeedback;
    public GameObject prefabNotEnoughGold;

    public List<Upgrade> upgrades;
    public GameObject prefabUpgradeUI;
    public GameObject parentUpgrades; //objet "content"

    List<GameObject> nonpermanentUpgrade = new List<GameObject>();
    List<Upgrade> _unlockedUpgrades = new List<Upgrade>(); //Liste d'upgrades d�bloqu�es
    float _timerAutoDamage;

    public static MainGame Instance; //permet d'acc�der � MainGame partout

    public int goldmoney = 0; //Monnaie
    public TextMeshProUGUI goldText;
    [SerializeField] int mousedamage = 1;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        monster.SetMonster(monsters[_currentMonster]);
        GetMonster();
        UpdateGold(goldmoney);

        //G�n�ration des upgrades
        foreach (var upgrade in upgrades)
        {
            if (upgrade.ispermanent == false)
            {
                GameObject go = GameObject.Instantiate(prefabUpgradeUI, parentUpgrades.transform, false);
                go.transform.localPosition = Vector3.zero;
                go.GetComponent<UpgradeUI>().Initialize(upgrade);

                nonpermanentUpgrade.Add(go);
            }
        }
    }

    void Update()
    {
        ///////////////D�g�ts des am�liorations achet�es/////////////////
        _timerAutoDamage += Time.deltaTime; //Timer
        if (_timerAutoDamage >= 1.0f)
        {
            _timerAutoDamage = 0;
            foreach (var upgrade in _unlockedUpgrades)
            {
                Hit(upgrade.DPS, monster);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            if (hit.collider != null)
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                Hit(mousedamage, monster); //d�g�ts de clic de la souris
            }
        }
    }

    void GetMonster()
    {
        GameObject monster = GameObject.FindGameObjectWithTag("Monster");
        monsterMaxHP = monster.GetComponent<Monster>().maxHP;
        monsterHP = monsterMaxHP;
        Debug.Log("Vie du monstre :" + monsterHP);
    }

    private void NextMonster()
    {
        _currentMonster++;
        monster.SetMonster(monsters[_currentMonster]);

        Debug.Log("Prochain monstre !");
    }

    void Hit(int damage, Monster monster) //L'ennemi prend des d�g�ts
    {
        monster.Hit(damage);

        //Feedback des d�g�ts inflig�s
        GameObject go = GameObject.Instantiate(prefabDamageFeedback, monster.transform, false);
        go.GetComponent<TextMeshProUGUI>().text = "" + damage;
        GameObject.Destroy(go, 1);

        if (go != null)
        {
            go.transform.localPosition = UnityEngine.Random.insideUnitCircle * 1.5f;
            go.transform.DOLocalMoveY(Random.Range(2, 7), 0.8f);
            go.transform.DOLocalMoveX(Random.Range(-3, 3), 0.8f);
            go.GetComponent<TextMeshProUGUI>().DOFade(0, 0.8f);
        }

        if (monster.isAlive() == false)
        {
            goldmoney += monster.monsterReward;
            UpdateGold(goldmoney);
            NextMonster();
        }
    }

    public void AddUpgrade(Upgrade upgrade) //Ajoute l'am�lioration � la liste des am�liorations d�bloqu�es
    {
        _unlockedUpgrades.Add(upgrade);
        //DEBUG//
        string unlockedupdates = "Upgrades actuelles : ";
        for (int i = 0; i < _unlockedUpgrades.Count; i++)
        {
            unlockedupdates += _unlockedUpgrades[i].name + ", ";
        }
        Debug.Log(unlockedupdates);
    }

    public void DeleteNonPermanent(Upgrade upgrade)
    {
        if(nonpermanentUpgrade.Count != 0)
        {
            foreach(var item in nonpermanentUpgrade)
            {
                if(item.GetComponent<UpgradeUI>().textName.text == upgrade.name)
                {
                    item.GetComponent<UpgradeUI>().gameObject.SetActive(false);
                }
            }
        }
    }

    public void UpdateGold(int gold) //Update le texte
    {
        goldText.GetComponent<TextMeshProUGUI>().text = "Gold : " + gold;
    }
}
