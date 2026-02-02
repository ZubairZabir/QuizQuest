using UnityEngine;

public class Pipes : MonoBehaviour
{
    public Transform top;
    public Transform bottom;
    public float speed = 5f;
    public float gap = 3f;

    private float leftEdge;
    private GameObject scoringZone;

    private void Start()
    {
        // Try to find top and bottom transforms if not assigned
        if (top == null) {
            Transform topTransform = transform.Find("Top");
            if (topTransform == null) {
                topTransform = transform.Find("top");
            }
            top = topTransform;
        }
        
        if (bottom == null) {
            Transform bottomTransform = transform.Find("Bottom");
            if (bottomTransform == null) {
                bottomTransform = transform.Find("bottom");
            }
            bottom = bottomTransform;
        }
        
        // Only adjust positions if transforms are assigned
        if (top != null && bottom != null) {
            leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
            top.position += Vector3.up * gap / 2;
            bottom.position += Vector3.down * gap / 2;
            
            // Create scoring zone automatically if it doesn't exist
            CreateScoringZone();
        } else {
            Debug.LogError($"Pipes script on {gameObject.name}: 'top' or 'bottom' Transform is not assigned and could not be found automatically. Please assign them in the Inspector or ensure child objects named 'Top'/'top' and 'Bottom'/'bottom' exist.");
        }
    }
    
    private void CreateScoringZone()
    {
        // Check if scoring zone already exists
        scoringZone = transform.Find("ScoringZone")?.gameObject;
        
        if (scoringZone == null) {
            // Create scoring zone GameObject
            scoringZone = new GameObject("ScoringZone");
            scoringZone.transform.SetParent(transform);
            scoringZone.transform.localPosition = Vector3.zero;
            
            // Position it between top and bottom pipes
            float middleY = (top.position.y + bottom.position.y) / 2f;
            scoringZone.transform.position = new Vector3(transform.position.x, middleY, transform.position.z);
            
            // Add BoxCollider2D as trigger
            BoxCollider2D collider = scoringZone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(0.5f, gap * 2f); // Make it slightly wider than the gap
            
            // Tag it as Scoring
            scoringZone.tag = "Scoring";
            
            // Add a component to track if this zone has been scored
            ScoringZoneTracker tracker = scoringZone.AddComponent<ScoringZoneTracker>();
            
            // Debug.Log("Created scoring zone for pipes: " + gameObject.name);
        } else {
            // Make sure existing scoring zone is set up correctly
            BoxCollider2D collider = scoringZone.GetComponent<BoxCollider2D>();
            if (collider == null) {
                collider = scoringZone.AddComponent<BoxCollider2D>();
            }
            collider.isTrigger = true;
            scoringZone.tag = "Scoring";
            
            // Ensure tracker component exists
            if (scoringZone.GetComponent<ScoringZoneTracker>() == null) {
                scoringZone.AddComponent<ScoringZoneTracker>();
            }
        }
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * Vector3.left;

        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }

}
