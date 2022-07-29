using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : Singleton<UIManager>
{
    [HideInInspector]
    public Level level;
    [HideInInspector]
    public Player player;

    public MiniMap miniMap;

    public Slider bossHp;

    public HP hp;
    public Text num;
    public Text Coin;
    public Image weapon;


    // public PausePanel pausePanel;

    void Start()
    {
        level = GameManager.Instance.level;
        player = GameManager.Instance.player;
        hp.player = player;
        miniMap.level = level;
        num.text = GameManager.Instance.depth.ToString();
    }

    public void initialize()
    {
        level = GameManager.Instance.level;
        player = GameManager.Instance.player;
        hp.player = player;
        miniMap.level = level;
        num.text = GameManager.Instance.depth.ToString();

    }

    public void PlayerUIInitialize()
    {
        hp.UpdateHP();
        UpdateStatus();
    }

    public void UpdateStatus()
    {
        Coin.text = player.coins.ToString();
        weapon.sprite = player.GetGunNow().GetComponent<SpriteRenderer>().sprite;
        weapon.transform.Find("WeaponName").GetComponent<Text>().text = player.GetGunNow().name.ToString();
    }
}

