using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    // Start is called before the first frame update
  
    public void ReturnHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    }
    
    public void MultiplayerScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MultiPlayer");
    }
    
    
}
