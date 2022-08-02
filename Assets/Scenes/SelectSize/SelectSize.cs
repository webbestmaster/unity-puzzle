using UnityEngine;
using UnityEngine.SceneManagement;

struct SavedDataKey
{
    public const string HeightInUnit = "heightInUnit";
    public const string WidthInUnit = "widthInUnit";
    public const string SpriteName = "";
}

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
        PlayerPrefs.SetInt(SavedDataKey.HeightInUnit, heightInUnit);
        PlayerPrefs.SetInt(SavedDataKey.WidthInUnit, widthInUnit);
        
        SceneManager.LoadScene("SelectImage");
    }
}
