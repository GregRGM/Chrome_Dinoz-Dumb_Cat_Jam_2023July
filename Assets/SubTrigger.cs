using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SubTrigger : MonoBehaviour
{
    public delegate void TriggerEntered(Collider2D c);
    public event TriggerEntered onTriggerEntered;

    public delegate void TriggerExited(Collider2D c);
    public event TriggerExited onTriggerExited;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEntered?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerExited?.Invoke(collision);
    }
}
