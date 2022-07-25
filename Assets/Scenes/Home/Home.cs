using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 30;
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public static void loadGmae()
    {
        SceneManager.LoadScene("Game");
    }
    
}
