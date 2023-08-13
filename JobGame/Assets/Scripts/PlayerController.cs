using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform leftColumn;
    public Transform rightColumn;
    public GameObject blockPrefab;
    public float columnSpeed = 2f;
    public float blockSpeed = 5f;
    public float blockSpawnRate = 1f;
    public float PlusHeightRight;
    public float PlusHeightLeft;
    public Sprite[] skins;
    private Collider2D col;
    private bool firstBlockPlace = false;
    private bool firstBlock = true;


    public bool isOnLeftColumn = true;
    private GameObject cam;

    private float jumpDuration = 0.5f;
    public bool BlockPlace = true;

    public bool CanJump = false;
    public bool CanDie = false;
    public GameManager gm;
    private BlockController BC;

    public bool firsJump = true;

    private void Start()
    {
        InvokeRepeating("SpawnBlock", 0f, blockSpawnRate);
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        gameObject.GetComponent<SpriteRenderer>().sprite = skins[Random.Range(0, 6)];
        Time.timeScale = 1f;
        col = gameObject.GetComponent<Collider2D>();

        Invisibility();
        Respawn();
    }

    private void Update()
    {
        if (GameManager.IsGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            cam.transform.Translate(Vector3.up * columnSpeed * Time.deltaTime);
        }
        if (transform.position.x <= -2)
        {
            transform.position = new Vector3(leftColumn.transform.position.x, transform.position.y + 10 + PlusHeightLeft, transform.position.z);
        }
        else if (transform.position.x >= 2)
        {
            transform.position = new Vector3(rightColumn.transform.position.x, transform.position.y + 10 + PlusHeightLeft, transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            GameManager.DeleteAllData();
        }
    }

    private void MoveColumns()
    {
        if (leftColumn.position.y >= rightColumn.position.y)
        {
            rightColumn.position = new Vector3(rightColumn.position.x, leftColumn.position.y);
        }
        else
        {
            leftColumn.position = new Vector3(leftColumn.position.x, rightColumn.position.y);
        }
    }

    private void SpawnBlock()
    {
        if (GameManager.IsGameStarted == true)
        {

            float randomX;
            Vector3 targetPosition;


            if (firstBlock)
            {
                if (firstBlockPlace) BlockPlace = false;
                if (BlockPlace)
                {
                    randomX = 4;
                    targetPosition = new Vector3(leftColumn.position.x, leftColumn.position.y + 2 + PlusHeightLeft);
                    BlockPlace = false;
                    PlusHeightLeft += 0.8f;
                    firstBlockPlace = false;
                    firstBlock = false;
                }
                else
                {
                    randomX = -4;
                    targetPosition = new Vector3(rightColumn.position.x, rightColumn.position.y + 2 + PlusHeightRight);
                    BlockPlace = true;
                    PlusHeightRight += 0.8f;
                    firstBlockPlace = false;
                    firstBlock = false;
                }
            }
            else
            {
                if (BlockPlace)
                {
                    randomX = 4;
                    targetPosition = new Vector3(leftColumn.position.x, leftColumn.position.y + 2 + PlusHeightLeft);
                    BlockPlace = false;
                    PlusHeightLeft += 0.8f;
                }
                else
                {
                    randomX = -4;
                    targetPosition = new Vector3(rightColumn.position.x, rightColumn.position.y + 2 + PlusHeightRight);
                    BlockPlace = true;
                    PlusHeightRight += 0.8f;
                }
            }

            GameObject block = Instantiate(blockPrefab, new Vector3(randomX, leftColumn.position.y + 2 + PlusHeightRight, 0f), Quaternion.identity);
            block.GetComponent<BlockController>().MoveToTarget(targetPosition, blockSpeed);
            block.GetComponent<BlockController>().CanDieInThisBlock = true;
            Debug.Log("Bloc Spawned");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hit") && CanDie)
        {
            BC = collision.gameObject.transform.parent.GetComponent<BlockController>();
            if (BC.CanDieInThisBlock)
            {
                gm.SetlastTime();
                gm.Loose();
                Debug.Log("Loose");
            }
            BC.CanDieInThisBlock = false;
        }

        if (collision.gameObject.CompareTag("Platform"))
        {
            CanJump = true;
            CanDie = true;
        }
    }
    public void Jump()
    {
        if (firsJump == true)
        {
            firsJump = false;
        }
        else
        {
            if (CanJump)
            {
                Vector3 targetPosition;

                if (isOnLeftColumn)
                {
                    targetPosition = new Vector3(rightColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 5)));
                    targetPosition = new Vector3(rightColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 20)));
                    isOnLeftColumn = false;
                    firstBlockPlace = true;
                }
                else
                {
                    targetPosition = new Vector3(leftColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 5)));
                    targetPosition = new Vector3(leftColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 20)));
                    isOnLeftColumn = true;
                    firstBlockPlace = false;
                }

                StartCoroutine(JumpRoutine(targetPosition));
                CanJump = false;
            }
        }
    }

    private IEnumerator JumpRoutine(Vector3 targetPosition)
    {
        float jumpHeight = 2f + (GameManager.JumpLvl / 2);
        float jumpDuration = 1f;

        Vector3 startingPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < jumpDuration)
        {
            float t = elapsedTime / jumpDuration;
            float yOffset = CalculateBallisticJumpHeight(t, jumpHeight);
            Vector3 newPosition = Vector3.Lerp(startingPosition, targetPosition, t) + Vector3.up * yOffset;
            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }


    private float CalculateBallisticJumpHeight(float t, float jumpHeight)
    {
        float yOffset = jumpHeight * (4f * t - 4f * t * t);
        return yOffset;
    }
    IEnumerator ToggleCollider()
    {
        col.enabled = false;
        yield return new WaitForSeconds(0.1f);
        col.enabled = true;
    }

    public void Invisibility()
    {
        StartCoroutine(ToggleCollider());
        CanDie = false;
    }
    public void Respawn()
    {
        if (CanJump)
        {
            if (BlockPlace)
            {
                transform.position = new Vector3(leftColumn.transform.position.x, transform.position.y + 7, transform.position.z);
                isOnLeftColumn = true;
            }
            else
            {
                transform.position = new Vector3(rightColumn.transform.position.x, transform.position.y + 7, transform.position.z);
                isOnLeftColumn = false;
            }
        }
        else
        {
            if (BlockPlace)
            {
                transform.position = new Vector3(leftColumn.transform.position.x, transform.position.y + 7, transform.position.z);
                isOnLeftColumn = true;
            }
            else
            {
                transform.position = new Vector3(rightColumn.transform.position.x, transform.position.y + 7, transform.position.z);
                isOnLeftColumn = false;
            }
        }
    }


}