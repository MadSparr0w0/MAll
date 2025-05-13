using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    public GameObject Panel;
    public Text Text;
    public KeyCode interactKey = KeyCode.F;
    public string[] hints;
    public float interactionDistance = 3f;

    private int currentHintIndex = 0;
    private bool isPlayerNear = false;
    private Transform player;
    private bool allHintsViewed = false;
    private bool isFirstInteraction = true;

    void Start()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);

            if (Text == null)
            {
                Text = Panel.GetComponentInChildren<Text>();
            }
        }

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || Panel == null || Text == null || allHintsViewed) return;

        float distance = Vector3.Distance(transform.position, player.position);
        isPlayerNear = distance <= interactionDistance;

        if (isPlayerNear)
        {
            if (!Panel.activeSelf && hints.Length > 0 && isFirstInteraction)
            {
                Show(0);
                isFirstInteraction = false;
            }

            if (Input.GetKeyDown(interactKey) && Panel.activeSelf)
            {
                ShowNext();
            }
        }
        else if (Panel.activeSelf)
        {
            Hide();
        }
    }

    void Show(int index)
    {
        if (hints.Length == 0 || Text == null) return;

        currentHintIndex = index;
        Text.text = hints[index];
        Panel.SetActive(true);
    }

    void ShowNext()
    {
        if (hints.Length == 0 || Text == null) return;

        currentHintIndex = (currentHintIndex + 1) % hints.Length;
        Text.text = hints[currentHintIndex];

        if (currentHintIndex == 0)
        {
            allHintsViewed = true;
            Hide();
        }
    }

    void Hide()
    {
        if (Panel != null)
        {
            Panel.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
    public void ResetHints()
    {
        allHintsViewed = false;
        isFirstInteraction = true;
    }
}