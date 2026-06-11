using UnityEngine;

public class ShowInteraction : MonoBehaviour
{
    [SerializeField]
    private Canvas interactionUI;

    private void Awake()
    {
        interactionUI.gameObject.SetActive(false);
    }

    public void Show()
    {
        interactionUI.gameObject.SetActive(true);
    }

    public void Hide()
    {
        interactionUI.gameObject.SetActive(false);
    }
}
