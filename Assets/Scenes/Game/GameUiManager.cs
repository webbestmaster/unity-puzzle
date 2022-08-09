using Unity.VisualScripting;
using UnityEngine;

public class GameUiManager : MonoBehaviour
{
    public ModelValue<bool> gameOverPopupOpen = new ModelValue<bool>(false);

    private void Start()
    {
        gameOverPopupOpen.AddListener(() =>
        {
            Debug.Log("123");
        });

        gameOverPopupOpen.SetValue(true);
        
        Debug.Log(gameOverPopupOpen.GetValue());
    }

    void OnDestroy()
    {
        gameOverPopupOpen.OnDestroy(); 
    }
}
