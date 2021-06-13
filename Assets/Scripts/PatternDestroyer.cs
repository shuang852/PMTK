using UnityEngine;

public class PatternDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DestroyPatternTrigger"))
        {
            Destroy(other.transform.parent.gameObject);
        }
        else if (!other.CompareTag("Payload"))
        {
            Destroy(other.gameObject);
        }   
    }
}
