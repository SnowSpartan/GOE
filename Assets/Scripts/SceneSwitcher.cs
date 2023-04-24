using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string nextScene;

    // Update is called once per frame
    public void LoadScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
