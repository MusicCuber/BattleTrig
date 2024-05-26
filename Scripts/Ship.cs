using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Ship : MonoBehaviour
{
    [SerializeField] float spd, tolerance;
    [SerializeField] Sprite n, ne, e, se, s, sw, w, nw;
    [SerializeField] UnityEvent<Transform> onPlaceCheckpoint;
    [SerializeField] TextMeshProUGUI tmp;

    Vector3 oldMovement;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        oldMovement = Vector3.zero;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        PlaceCheckpoints();
    }

    void Movement()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) movement += Vector3.up;
        if (Input.GetKey(KeyCode.S)) movement += Vector3.down;
        if (Input.GetKey(KeyCode.D)) movement += Vector3.right;
        if (Input.GetKey(KeyCode.A)) movement += Vector3.left;
        oldMovement = (movement + oldMovement).normalized;
        transform.Translate(movement * Time.deltaTime * spd);

        if (oldMovement.y > tolerance)
        {
            if (oldMovement.x > tolerance) sr.sprite = ne;
            else if (oldMovement.x < -tolerance) sr.sprite = nw;
            else sr.sprite = n;
        }
        else if (oldMovement.y < -tolerance)
        {
            if (oldMovement.x > tolerance) sr.sprite = se;
            else if (oldMovement.x < -tolerance) sr.sprite = sw;
            else sr.sprite = s;
        }
        else
        {
            if (oldMovement.x > tolerance) sr.sprite = e;
            else if (oldMovement.x < -tolerance) sr.sprite = w;
        }

        tmp.SetText($"{Mathf.RoundToInt(transform.position.x)}, {Mathf.RoundToInt(transform.position.y)}");
    }

    void PlaceCheckpoints()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        onPlaceCheckpoint?.Invoke(transform);
    }
}
