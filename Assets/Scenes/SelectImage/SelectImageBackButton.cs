using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectImageBackButton : MonoBehaviour
{
    public void HandleBackButton()
    {
        SceneManager.LoadScene("SelectSize");
    } 
}
