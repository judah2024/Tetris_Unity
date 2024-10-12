using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void LoadTitle()
    {
        SceneManager.LoadScene("Scenes/Title");
    }
}
