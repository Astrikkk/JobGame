using UnityEngine;

public class BlockController : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed;
    private Collider2D col;
    public GameObject Hit;
    public Sprite placed;
    public bool CanDieInThisBlock = true;

    public static int newRec = 0;

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

        MoveOnOPos();
        if (targetPosition == null) Destroy(gameObject);
    }
    private void MoveOnOPos()
    {
        if (transform.position == targetPosition)
        {
            enabled = false;
            col.isTrigger = false;
            GameManager.Money += GameManager.Income;
            Destroy(Hit);
            GameManager.Score++;
            if (GameManager.Score > GameManager.BestScore)
            {
                GameManager.BestScore = GameManager.Score;
                if (newRec == 0)
                    newRec = 1;
            }
            gameObject.GetComponent<SpriteRenderer>().sprite = placed;
        }
    }
}
