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

    private bool isOnLeftColumn = true; // Змінна, що вказує, чи персонаж знаходиться на лівому стовбі
    private GameObject cam;

    private float jumpDuration = 0.5f; // Тривалість прижка
    private bool BlockPlace = true;

    private bool CanJump = false;

    private void Start()
    {
        InvokeRepeating("SpawnBlock", 0f, blockSpawnRate);
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        gameObject.GetComponent<SpriteRenderer>().sprite = skins[Random.Range(0, 6)];
        Time.timeScale = 1f;

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

            if (BlockPlace)
            {
                randomX = 5;
                targetPosition = new Vector3(leftColumn.position.x, leftColumn.position.y + 2f + PlusHeightLeft);
                BlockPlace = false;
                PlusHeightLeft += 0.8f;
            }
            else
            {
                randomX = -5;
                targetPosition = new Vector3(rightColumn.position.x, rightColumn.position.y + 1.5f + PlusHeightRight);
                BlockPlace = true;
                PlusHeightRight += 0.8f;
            }

            GameObject block = Instantiate(blockPrefab, new Vector3(randomX, leftColumn.position.y + 1 + PlusHeightRight, 0f), Quaternion.identity);
            block.GetComponent<BlockController>().MoveToTarget(targetPosition, blockSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Hit"))
        {
            GameManager.RestartScene();
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
            }
            else
            {
                targetPosition = new Vector3(leftColumn.position.x, transform.position.y + (2 + (GameManager.JumpLvl / 2)));
                isOnLeftColumn = true;
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

    // Розраховує висоту прижку по балістичній траєкторії
    private float CalculateBallisticJumpHeight(float t, float jumpHeight)
    {
        float yOffset = jumpHeight * (4f * t - 4f * t * t);
        return yOffset;
    }



}