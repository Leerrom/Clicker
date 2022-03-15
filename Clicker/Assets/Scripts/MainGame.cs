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

    public List<Upgrade> upgrades; //permet de rentrer dans l'editor les upgrades
    public GameObject prefabUpgradeUI;
    public GameObject parentUpgrades; //objet "content"

    List<GameObject> permanentUpgrade = new List<GameObject>(); //Liste les améliorations permanentes
    List<GameObject> nonpermanentUpgrade = new List<GameObject>(); //Liste les améliorations non-permanentes
    List<Upgrade> _unlockedUpgrades = new List<Upgrade>(); //Liste d'upgrades débloquées
    float _timerAutoDamage;

    public static MainGame Instance; //permet d'accéder à MainGame partout

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

        //Génération des upgrades
        foreach (var upgrade in upgrades)
        {
            GameObject go = GameObject.Instantiate(prefabUpgradeUI, parentUpgrades.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<UpgradeUI>().Initialize(upgrade);

            if (upgrade.ispermanent == false)
            {
                nonpermanentUpgrade.Add(go);
            }
            else
            {
                permanentUpgrade.Add(go);
                go.SetActive(false);
            }
        }

        //Liste des upgrades permanentes
        foreach (var item2 in permanentUpgrade)
        {
            Debug.Log(item2.GetComponent<UpgradeUI>().textName.text);
        }
    }

    void Update()
    {
        ///////////////Dégâts des améliorations achetées/////////////////
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
                Hit(mousedamage, monster); //dégâts de clic de la souris
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
    }

    void Hit(int damage, Monster monster) //L'ennemi prend des dégâts
    {
        monster.Hit(damage);

        //Feedback des dégâts infligés
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

    public void AddUpgrade(Upgrade upgrade) //Ajoute l'amélioration à la liste des améliorations débloquées
    {
        _unlockedUpgrades.Add(upgrade);
        //DEBUG//
        string unlockedupdates = "Upgrades actuelles : ";
        for (int i = 0; i < _unlockedUpgrades.Count; i++)
        {
            unlockedupdates += ", " + _unlockedUpgrades[i].name;
        }
        Debug.Log(unlockedupdates);
    }

    public void DeleteNonPermanent(Upgrade upgrade) //Supprime les améliorations non-permanentes et génère les permanentes
    {
        if(nonpermanentUpgrade.Count != 0)
        {
            foreach(var item in nonpermanentUpgrade)
            {
                if(item.GetComponent<UpgradeUI>().textName.text == upgrade.name)
                {
                    if (item.GetComponent<UpgradeUI>().textName.text == "The Knight")
                    {
                        Debug.Log("Knight upgrade");
                        foreach (var item2 in permanentUpgrade)
                        {
                            if (item2.GetComponent<UpgradeUI>().textName.text == "Upgrade Shield")
                            {
                                item2.SetActive(true);
                                break;
                            }
                        }
                    }
                    if (item.GetComponent<UpgradeUI>().textName.text == "The Doctor")
                    {
                        Debug.Log("Doctor upgrade");
                        foreach (var item2 in permanentUpgrade)
                        {
                            if (item2.GetComponent<UpgradeUI>().textName.text == "Upgrade Potion")
                            {
                                item2.SetActive(true);
                                break;
                            }
                        }
                    }
                    item.GetComponent<UpgradeUI>().gameObject.SetActive(false);
                }
            }
        }
    }

    public void GeneratePermanent(Upgrade upgrade)
    {
        
    }

    public void UpdateGold(int gold) //Update le texte
    {
        goldText.GetComponent<TextMeshProUGUI>().text = "Gold : " + gold;
    }
}
