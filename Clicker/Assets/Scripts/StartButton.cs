using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void ClickPlay()
    {
        SceneManager.LoadScene("02_Game");
    }
}
