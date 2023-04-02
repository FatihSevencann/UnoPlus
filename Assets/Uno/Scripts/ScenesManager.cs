using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    public void ReturnHome()=> UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScene");
    public void MultiplayerScene()=> UnityEngine.SceneManagement.SceneManager.LoadScene("MultiPlayer");
}
