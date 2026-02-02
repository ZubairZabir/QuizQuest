using UnityEngine;

public class ScoringZoneTracker : MonoBehaviour
{
    private bool hasScored = false;

    public bool HasScored()
    {
        return hasScored;
    }

    public void MarkAsScored()
    {
        hasScored = true;
        // Disable the collider to prevent multiple triggers
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) {
            col.enabled = false;
        }
    }
}
