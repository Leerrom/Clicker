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
    public GameObject prefabDamageFeedback;

    public List<Upgrade> upgrades;
    public GameObject prefabUpgradeUI;
    public GameObject parentUpgrades; //objet "content"

    List<Upgrade> _unlockedUpgrades = new List<Upgrade>();
    float _timerAutoDamage;

    void Start()
    {
        monster.SetMonster(monsters[_currentMonster]);
        GetMonster();

        //Génération des upgrades
        foreach (var upgrade in upgrades)
        {
            GameObject go = GameObject.Instantiate(prefabUpgradeUI, parentUpgrades.transform, false);
            go.transform.localPosition = Vector3.zero;
            go.GetComponent<UpgradeUI>().Initialize(upgrade);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(world, Vector2.zero);

            if (hit.collider != null)
            {
                Monster monster = hit.collider.GetComponent<Monster>();
                monster.Hit(1);

                if (monster.isAlive() == false)
                {
                    NextMonster();
                }

                //Feedback des dégâts infligés
                GameObject go = GameObject.Instantiate(prefabDamageFeedback, monster.transform, false);
                go.transform.localPosition = UnityEngine.Random.insideUnitCircle * 1.5f;
                go.transform.DOLocalMoveY(Random.Range(2, 7), 0.8f);
                go.transform.DOLocalMoveX(Random.Range(-3, 3), 0.8f);
                go.GetComponent<TextMeshProUGUI>().DOFade(0, 0.8f);
                GameObject.Destroy(go, 0.8f);
            }

            _timerAutoDamage += Time.deltaTime;

            if (_timerAutoDamage >= 1.0f)
            {
                _timerAutoDamage = 0;
                foreach (var upgrade in _unlockedUpgrades)
                {
                    monster.Hit(upgrade.DPS);
                }
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
}
