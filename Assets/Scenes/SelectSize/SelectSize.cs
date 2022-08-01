using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectSize : MonoBehaviour
{
    [SerializeField] private int heightInUnit = 0;
    [SerializeField] private int widthInUnit = 0;
    
    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    public void HandleSelectSize()
    {
        PlayerPrefs.SetInt("heightInUnit", heightInUnit);
        PlayerPrefs.SetInt("widthInUnit", widthInUnit);
        
        SceneManager.LoadScene("SelectImage");
    }
    
}
