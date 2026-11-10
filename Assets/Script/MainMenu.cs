using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Tentang()
    {
        SceneManager.LoadScene("Tentang");
    }

    public void Mulai()
    {
        SceneManager.LoadScene("Menu1");
    }

    public void MateriHPLC()
    {
        SceneManager.LoadScene("MateriHPLC");
    }

    public void Kuis()
    {
        SceneManager.LoadScene("Kuis");
    }

    public void Keluar()
    {
        Application.Quit();
    }
}