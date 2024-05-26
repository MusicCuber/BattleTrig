using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [SerializeField] float dampening;

    Vector3 velocity;
    Transform curTarg;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector3.zero;
        curTarg = target;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movePos = curTarg.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, movePos, ref velocity, dampening);
    }

    public void ZoomOut(LineRenderer lr)
    {
        Vector3 total = Vector3.zero;
        for (int i = 0; i < 3; i++)
        {
            total += lr.GetPosition(i);
        }
        total /= 3;

        GameObject newCenter = new GameObject();
        newCenter.transform.position = total;
        curTarg = newCenter.transform;

        GetComponent<Camera>().orthographicSize *= 1.5f;
    }

    public void ZoomIn()
    {
        GetComponent<Camera>().orthographicSize *= 2/3f;
        curTarg = target;
    }
}
