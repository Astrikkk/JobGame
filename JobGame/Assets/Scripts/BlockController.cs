using UnityEngine;

public class BlockController : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private Collider2D col;
    public GameObject Hit;
    public Sprite placed;

    private void Start()
    {
        col = gameObject.GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public void MoveToTarget(Vector3 target, float movementSpeed)
    {
        targetPosition = target;
        speed = movementSpeed;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            enabled = false;
            col.isTrigger = false;
            GameManager.Money += GameManager.Income;
            Destroy(Hit);
            gameObject.GetComponent<SpriteRenderer>().sprite = placed;
        }
    }
}
