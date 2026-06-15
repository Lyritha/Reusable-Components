using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseScreen : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;   
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void MainGame()
    {
        SceneManager.LoadScene(1);
    }
}
