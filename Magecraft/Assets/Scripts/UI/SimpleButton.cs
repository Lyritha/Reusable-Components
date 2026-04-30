using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour
{
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private Button button;

    public Button Button => button;

    public void SetTitle(string title)
    {
        if (titleText != null) titleText.text = title;
    }
}
