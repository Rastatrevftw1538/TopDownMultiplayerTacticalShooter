using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class StartMenuSP : MonoBehaviour
{
    //FOR NOW
    public void StartGame()
    {
        SceneManager.LoadScene("Assets/Scenes/Single Player/ArenaSinglePlayer.unity");
    }

    public void EndGame()
    {
        Application.Quit(0);
    }
}
