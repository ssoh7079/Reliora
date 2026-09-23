using UnityEngine;

public class ControlGuide : MonoBehaviour
{
    [SerializeField] private GameObject guidePanel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        guidePanel.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null) return;
        guidePanel.SetActive(false);
    }
}
