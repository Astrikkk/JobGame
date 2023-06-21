using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    private GameObject Mcam;
    public float speed;
    public Sprite[] sprites;
    private void Start()
    {
        Mcam = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y<=Mcam.transform.position.y - 20)
        {
            Respawm();
        }
    }

    public void Respawm()
    {
        int a = Random.Range(0, 5);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[a];
        transform.position = new Vector3(Random.Range(-4, 4), Mcam.transform.position.y + 10, 0);
    }
}
