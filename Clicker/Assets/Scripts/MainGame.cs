using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MainGame : MonoBehaviour
{
    //GameObject monster;
    int monsterMaxHP; //HP max
    int monsterHP; //HP actuelle du monstre
    public List<MonsterInfos> monsters;
    public MonsterInfos randomMonster;

    public Animator knightanimator;
    public Animator doctoranimator;

    public List<Sprite> monsterSprites;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    System.Random random = new System.Random();

    int _currentMonster; //index du monstre actuel
    public Monster monster;

    public Canvas canvas;
    public Canvas importantcanvas;
    public GameObject prefabDamageFeedback;
    public GameObject prefabNotEnoughGold;
    public GameObject prefabDoctor;
    public GameObject prefabKnight;
    public GameObject knightLVLvisual;
    public GameObject doctorLVLvisual;
    private int knightLVL = 1;
    private int doctorLVL = 1;

    public List<Upgrade> upgrades; //permet de rentrer dans l'editor les upgrades
    public GameObject prefabUpgradeUI;
    public GameObject parentUpgrades; //objet "content"
    public GameObject spawnKnight;
    public GameObject spawnDoctor;
    public GameObject spawnExplosion;
    public GameObject spawnCoins;
    public GameObject prefabExplosion;
    public GameObject spawnShieldbash;
    public GameObject prefabShieldbash;
    public GameObject prefabCoins;
    public GameObject prefabSlash;
    public GameObject prefabPouf;
    bool explosion = false;

    private Vector3 mousePos;
    private Vector3 objectPos;

    public List<GameObject> permanentUpgrade = new List<GameObject>(); //Liste les améliorations permanentes
    List<GameObject> nonpermanentUpgrade = new List<GameObject>(); //Liste les améliorations non-permanentes
    List<Upgrade> _unlockedUpgrades = new List<Upgrade>(); //Liste d'upgrades débloquées
    float _timerAutoDamage1;
    float _timerAutoDamage2;

    public static MainGame Instance; //permet d'accéder à MainGame partout

    public int goldmoney = 0; //Monnaie
    public TextMeshProUGUI goldText;
    public int mousedamage = 1;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _timerAutoDamage1 += 0.75f;

        monster.SetMonster(monsters[_currentMonster]);
        GetMonster();
        UpdateGold(goldmoney);
        monsterSprites.Add(sprite1);
        monsterSprites.Add(sprite2);
        monsterSprites.Add(sprite3);

        //Désactivation des visuels Knight et Doctor
        prefabKnight.SetActive(false);
        prefabDoctor.SetActive(false);
        knightLVLvisual.SetActive(false);
        doctorLVLvisual.SetActive(false);

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
                if (upgrade.name != "Upgrade Slash")
                {
                    go.SetActive(false);
                }
                else
                {
                    //Rajoute basiquement le Upgrade Slash dans la liste des améliorations débloquées
                    AddUpgrade(upgrade);
                }
            }
        }
    }

    void Update()
    {
        ///////////////Dégâts des améliorations achetées/////////////////
        _timerAutoDamage1 += Time.deltaTime; //Timer
        _timerAutoDamage2 += Time.deltaTime;

        //Timer Knight
        if (_timerAutoDamage1 >= 1.5f)
        {
            foreach (var upgrade in _unlockedUpgrades)
            {
                _timerAutoDamage1 = 0;
                if (upgrade.name == "The Knight")
                {
                    Hit(upgrade.DPS, monster);
                    knightanimator.SetBool("Attack", true);
                    Instantiate(prefabShieldbash, spawnShieldbash.transform);
                }
            }
        }
        if (_timerAutoDamage1 >= 0.4f)
        {
            knightanimator.SetBool("Attack", false);
        }


        //Timer Doctor
        if (_timerAutoDamage2 >= 2.5f && explosion == false)
        {
            foreach (var upgrade in _unlockedUpgrades)
            {
                if (upgrade.name == "The Doctor")
                {
                    Instantiate(prefabExplosion, spawnExplosion.transform);
                    doctoranimator.SetBool("Attack", true);
                    explosion = true;
                }
            }
        }
        if (_timerAutoDamage2 >= 2.9f)
        {
            doctoranimator.SetBool("Attack", false);
        }
        if (_timerAutoDamage2 >= 3f)
        {
            _timerAutoDamage2 = 0;
            foreach (var upgrade in _unlockedUpgrades)
            {
                if (upgrade.name == "The Doctor")
                {
                    Hit(upgrade.DPS, monster);
                    explosion = false;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            mousePos = Input.mousePosition;
            objectPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3 pos = new Vector3(objectPos.x, objectPos.y, -0.5f);

            if (hit.collider != null)
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                Hit(mousedamage, monster); //dégâts de clic de la souris

                Instantiate(prefabSlash, pos, Quaternion.identity);
            }
        }
    }

    void GetMonster()
    {
        GameObject monster = GameObject.FindGameObjectWithTag("Monster");
        monsterMaxHP = monster.GetComponent<Monster>().maxHP;
        monsterHP = monsterMaxHP;
        //Debug.Log("Vie du monstre :" + monsterHP);
    }

    private void NextMonster()
    {
        monster.SetMonster(monsters[0]);
        Instantiate(prefabCoins, spawnCoins.transform);

        //Animation
        monster.transform.position = new Vector3(monster.transform.position.x+4, monster.transform.position.y, monster.transform.position.z);
        monster.transform.DOLocalMoveX(6.87f, 0.5f);
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
            GenerateMonsterInfos();
            goldmoney += monster.monsterReward;
            UpdateGold(goldmoney);
            monsters.RemoveAt(0);
            NextMonster();

            //DisplayMonsterList();
        }
    }

    public void AddUpgrade(Upgrade upgrade) //Ajoute l'amélioration à la liste des améliorations débloquées
    {
        _unlockedUpgrades.Add(upgrade);
        //DisplayUnlockedUpdates();
    }

    public void DeleteNonPermanent(Upgrade upgrade) //Supprime les améliorations non-permanentes et génère les permanentes
    {
        AddUpgrade(upgrade);
        if(nonpermanentUpgrade.Count != 0)
        {
            foreach(var item in nonpermanentUpgrade)
            {
                if(item.GetComponent<UpgradeUI>().textName.text == upgrade.name)
                {
                    item.GetComponent<UpgradeUI>().gameObject.SetActive(false); //Fait disparaître l'amélioration

                    GeneratePermanentUpgrade(item, "The Knight", "Upgrade Shield"); //Génère la nouvelle amélioration permanente du Knight
                    GeneratePermanentUpgrade(item, "The Doctor", "Upgrade Potion"); //Génère la nouvelle amélioration permanente du Doctor
                }
            }
        }
    }

    public void GeneratePermanentUpgrade(GameObject item, string nonpermupgrade, string permupgrade)
    {
        if (item.GetComponent<UpgradeUI>().textName.text == nonpermupgrade)
        {
            //Génère les améliorations permanentes
            foreach (var item2 in permanentUpgrade)
            {
                if (item2.GetComponent<UpgradeUI>().textName.text == permupgrade)
                {
                    item2.SetActive(true);
                    SpawnCharacter(nonpermupgrade);
                    break;
                }
            }
        }
    }

    public void SpawnCharacter(string charactertospawn)
    {
        if (charactertospawn == "The Knight")
        {
            prefabKnight.SetActive(true);
            Instantiate(prefabPouf, prefabKnight.transform);
            knightLVLvisual.SetActive(true);
        }
        if (charactertospawn == "The Doctor")
        {
            prefabDoctor.SetActive(true);
            Instantiate(prefabPouf, prefabDoctor.transform);
            doctorLVLvisual.SetActive(true);
        }
    }

    public void UpdateGold(int gold) //Update le texte
    {
        goldText.GetComponent<TextMeshProUGUI>().text = "Gold : " + gold;
    }

    public void PermanentUpgradeCheck(Upgrade upgrade) //Cherche quelle upgrade a été séléctionnée
    {
        Debug.Log(upgrade.name);
        string upgradename = upgrade.name;
        foreach (var item in _unlockedUpgrades)
        {
            if (upgradename == "Upgrade Shield")
            {
                UpgradeItem(upgrade, 40, "Upgrade Shield", "The Knight");
                Debug.Log("Amélioration du bouclier !");
                break;
            }
            if (upgradename == "Upgrade Potion")
            {
                UpgradeItem(upgrade, 35, "Upgrade Potion", "The Doctor");
                Debug.Log("Amélioration de la potion !");
                break;
            }
            if (upgradename == "Upgrade Slash")
            {
                Debug.Log("Amélioration du slash !");
                UpgradeSlash(upgrade, 35);
                break;
            }
        }
    }

    public void UpgradeItem(Upgrade upgrade, int upgradecost, string upgradename, string character)
    {
        upgrade.cost += upgradecost;
        
        foreach(var item in permanentUpgrade)
        {
            if (item.GetComponent<UpgradeUI>().textName.text == upgrade.name)
            {
                item.GetComponent<UpgradeUI>()._actualCost += upgradecost;
                break;
            }
        }

        if (character == "The Knight")
        {
            knightLVL++;
        }
        if (character == "The Doctor")
        {
            doctorLVL++;
        }
        UpdateCharacterLVL(character);

        RemoveUpgrade(upgradename);
        AddUpgrade(upgrade);

        UpdateCharacterDamage(upgrade.DPS, character);

        UpgradeUI.Instance.UpdateVisuals(upgrade, upgrade.cost);
        
        DisplayUnlockedUpdates();
    }

    public void UpgradeSlash(Upgrade upgrade, int upgradecost)
    {
        mousedamage++;
        upgrade.cost += upgradecost;
        UpgradeUI.Instance._actualCost += upgradecost;

        RemoveUpgrade("Upgrade Slash");
        AddUpgrade(upgrade);

        //Debug.Log(mousedamage);
        //Debug.Log(upgrade.cost);

        UpgradeUI.Instance.UpdateVisuals(upgrade, upgrade.cost);

        DisplayUnlockedUpdates();
    }

    public void UpdateVisual(GameObject visual, int dpsValue, int costValue)
    {
        visual.GetComponent<UnityEngine.UI.Text>().text = "" + dpsValue;
    }

    public void RemoveUpgrade(string upgradename)
    {
        //Supprime L'amélioration actuelle
        foreach (var item in _unlockedUpgrades)
        {
            if (item.name == upgradename)
            {
                _unlockedUpgrades.Remove(item);
                break;
            }
        }
    }

    public void UpdateCharacterDamage(int bonusDamage, string character) //Ajoute les dégâts de l'amélioration au DPS du personnage choisi
    {
        foreach (var item in _unlockedUpgrades)
        {
            if (item.name == character)
            {
                item.DPS += bonusDamage;
                //Debug.Log(item.name + " " + item.DPS);
                break;
            }
        }
    }

    public void GenerateMonsterInfos()
    {
        randomMonster.name = "Zombax";
        randomMonster.HP = monster.maxHP + random.Next(12,24);
        randomMonster.sprite = monsterSprites[random.Next(0, 3)];
        DisplayMonsterSprite();
        randomMonster.reward = randomMonster.HP * random.Next(1, 3);

        monsters.Add(randomMonster);
    }

    public void UpdateCharacterLVL(string character)
    {
        if (character == "The Knight")
        {
            knightLVLvisual.GetComponent<UnityEngine.UI.Text>().text = "LEVEL " + knightLVL;
        }
        if (character == "The Doctor")
        {
            doctorLVLvisual.GetComponent<UnityEngine.UI.Text>().text = "LEVEL " + doctorLVL;
        }
    }





    public void DisplayUnlockedUpdates()
    {
        string unlockedupdates = "Upgrades actuelles : ";
        for (int i = 0; i < _unlockedUpgrades.Count; i++)
        {
            unlockedupdates += ", " + _unlockedUpgrades[i].name;
        }
        Debug.Log(unlockedupdates);
    }

    public void DisplayMonsterList()
    {
        //Debug
        string monstersList = "Liste des monstres : ";
        for (int i = 0; i < monsters.Count; i++)
        {
            monstersList += ", " + monsters[i].name;
        }
        Debug.Log(monstersList);
    }

    public void DisplayMonsterSprite()
    {
        //Debug
        string monstersList = "Sprites des monstres : ";
        for (int i = 0; i < monsters.Count; i++)
        {
            monstersList += ", " + monsterSprites[i].name;
        }
        Debug.Log(monstersList);
    }
}
