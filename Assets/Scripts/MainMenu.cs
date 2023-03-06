using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static int initialDay = 0;
    public void Play (int day) {
        initialDay = day;
        SceneManager.LoadScene (1);
    }

    public void Credits () {

    }

    public void Quit () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
