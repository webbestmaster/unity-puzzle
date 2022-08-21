using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
    public void HandleBackButton()
    {
        SceneManager.LoadScene("SelectImage");
    } 
}
