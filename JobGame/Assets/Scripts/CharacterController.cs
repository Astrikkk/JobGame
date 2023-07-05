using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Transform leftColumn; // Посилання на лівий стовб
    public Transform rightColumn; // Посилання на правий стовб
    public GameObject blockPrefab; // Префаб блоку
    public float columnSpeed = 2f; // Швидкість руху стовбів
    public float blockSpeed = 5f; // Швидкість руху блоків
    public float blockSpawnRate = 1f; // Частота появи блоків
    public float PlusHeightRight;
    public float PlusHeightLeft;
    public Sprite[] skins;
    private Collider2D col;

    private bool isOnLeftColumn = true; // Змінна, що вказує, чи персонаж знаходиться на лівому стовбі
    private GameObject cam;

    private float jumpDuration = 0.5f; // Тривалість прижка
    public bool BlockPlace = true;
    private bool firstBlockPlace = false;
    private bool firstBlock = true;
    private float BlockPlacePlus = 2f;

    private bool CanJump = false;
    public GameManager gm;

    private void Start()
    {
        InvokeRepeating("SpawnBlock", 0f, blockSpawnRate);
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        gameObject.GetComponent<SpriteRenderer>().sprite = skins[Random.Range(0, 6)];
        Time.timeScale = 1f;
        col = gameObject.GetComponent<Collider2D>();
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
            transform.position = new Vector3(leftColumn.transform.position.x, transform.position.y + 10+ PlusHeightLeft, transform.position.z);
        }
        else if(transform.position.x >= 2)
        {
            transform.position = new Vector3(rightColumn.transform.position.x, transform.position.y + 10+ PlusHeightLeft, transform.position.z);
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
                    randomX = 5;
                    targetPosition = new Vector3(leftColumn.position.x, leftColumn.position.y + BlockPlacePlus + PlusHeightLeft);
                    BlockPlace = false;
                    PlusHeightLeft += 0.8f;
                    BlockPlacePlus = 1.5f;
                    firstBlockPlace = false;
                    firstBlock = false;
                }
                else
                {
                    randomX = -5;
                    targetPosition = new Vector3(rightColumn.position.x, rightColumn.position.y + BlockPlacePlus + PlusHeightRight);
                    BlockPlace = true;
                    PlusHeightRight += 0.8f;
                    BlockPlacePlus = 1.5f;
                    firstBlockPlace = false;
                    firstBlock = false;
                }
            }
            else
            {
                if (BlockPlace)
                {
                    randomX = 5;
                    targetPosition = new Vector3(leftColumn.position.x, leftColumn.position.y + BlockPlacePlus + PlusHeightLeft);
                    BlockPlace = false;
                    BlockPlacePlus = 1.5f;
                    PlusHeightLeft += 0.8f;
                }
                else
                {
                    randomX = -5;
                    targetPosition = new Vector3(rightColumn.position.x, rightColumn.position.y + BlockPlacePlus + PlusHeightRight);
                    BlockPlace = true;
                    BlockPlacePlus = 1.5f;
                    PlusHeightRight += 0.8f;
                }
            }

            GameObject block = Instantiate(blockPrefab, new Vector3(randomX, leftColumn.position.y + 1 + PlusHeightRight, 0f), Quaternion.identity);
            block.GetComponent<BlockController>().MoveToTarget(targetPosition, blockSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hit"))
        {
            gm.Loose();
        }
        if (collision.gameObject.CompareTag("Platform"))
        {
            CanJump = true;
        }
    }
    public void Jump()
    {
        if (CanJump)
        {
            Vector3 targetPosition;

            if (isOnLeftColumn)
            {
                targetPosition = new Vector3(rightColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 2)));
                isOnLeftColumn = false;
                firstBlockPlace = true;
            }
            else
            {
                targetPosition = new Vector3(leftColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 2)));
                isOnLeftColumn = true;
                firstBlockPlace = false;
            }

            StartCoroutine(JumpRoutine(targetPosition));
            CanJump = false;
        }
    }

    private IEnumerator JumpRoutine(Vector3 targetPosition)
    {
        float jumpHeight = 2f + (GameManager.JumpLvl / 2); // Висота прижку
        float jumpDuration = 1f; // Тривалість прижку

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
    }


}