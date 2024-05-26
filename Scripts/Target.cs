using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;

public class Target : MonoBehaviour
{
    [SerializeField] GameObject prefabCheckpoint, prefabLineText, questionText, prefabCannonball;
    [SerializeField] UnityEvent<LineRenderer> onLastCheckpointChecked;
    [SerializeField] UnityEvent onResumeControl;
    [SerializeField] TextMeshProUGUI tmp, coords, input;
    [SerializeField] TextAsset json;
    [SerializeField] Transform ship;

    LineRenderer lr;
    Triangles triangles;
    Triangle curTriangle;

    int triangleIndex;

    // Start is called before the first frame update
    void Start()
    {
        triangles = JsonUtility.FromJson<Triangles>(json.text);
        triangleIndex = 0;
        curTriangle = triangles.TriangleArr[0];
        transform.position = curTriangle.Vectors[0];

        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + Vector3.forward);

        LoadTriangle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadTriangle()
    {
        if (triangleIndex >= triangles.TriangleArr.Length) return;
        curTriangle = triangles.TriangleArr[triangleIndex];
        coords.SetText(string.Join(", ", curTriangle.Vectors));
    }

    IEnumerator ResetTriangle()
    {
        yield return new WaitForSeconds(1);
        bool isSame = true;
        for (int i = 0; i < curTriangle.Vectors.Length; i++)
        {
            if (lr.positionCount <= i) continue;
            if (curTriangle.Vectors[i] != (Vector2)lr.GetPosition(i)) isSame = false;
        }

        if (isSame)
        {
            AskQuestion();
            tmp.enabled = true;
            input.enabled = true;
        }
        else
        {
            DestroyTriangle();
            onResumeControl?.Invoke();
        }
    }

    void DestroyTriangle()
    {
        lr.positionCount = 1;
        foreach (GameObject gm in FindObjectsOfType<GameObject>())
        {
            if (!gm.name.Contains("Checkpoint") && !gm.name.Contains("LineText")) continue;
            Destroy(gm);
        }
    }

    void SetName(GameObject checkpoint)
    {
        TextMeshProUGUI tmp = checkpoint.GetComponentInChildren<TextMeshProUGUI>();
        switch (lr.positionCount)
        {
            case 0: tmp.SetText("A"); break;
            case 1: tmp.SetText("B"); break;
            case 2: tmp.SetText("C"); break;
        }
    }

    void SetLineName(GameObject line)
    {
        TextMeshProUGUI tmp = line.GetComponentInChildren<TextMeshProUGUI>();
        switch (lr.positionCount)
        {
            case 2: tmp.SetText("c"); break;
            case 3: tmp.SetText("a"); break;
            case 4: tmp.SetText("b"); break;
        }
    }

    void AfterPlace(Vector3 position)
    {
        Vector3Int pos = Vector3Int.RoundToInt(position + Vector3.forward);
        for (int i = 0; i < lr.positionCount; i++)
        {
            if (pos == lr.GetPosition(i) && lr.positionCount < 3) return;
        }
        if (lr.positionCount < 3)
        {
            GameObject point = Instantiate(prefabCheckpoint, pos, Quaternion.identity);
            SetName(point);
        }
        lr.positionCount++;
        lr.SetPosition(lr.positionCount - 1, pos);

        Vector3 pos0 = lr.GetPosition(lr.positionCount - 2);
        Vector3 pos1 = lr.GetPosition(lr.positionCount - 1);
        GameObject line = Instantiate(prefabLineText, (pos1 + pos0) / 2, Quaternion.identity);
        SetLineName(line);
    }

    void AskQuestion()
    {
        questionText.SetActive(true);
        tmp.SetText(curTriangle.Question);
    }

    public void CheckAnswer()
    {
        if (input.text.Substring(0, input.text.Length - 1) != curTriangle.Answer)
        {
            input.SetText("Wrong, try again");
        }
        else
        {
            questionText.SetActive(false);
            StartCoroutine(Cannon());
        }
    }

    IEnumerator Cannon()
    {
        GameObject cb = Instantiate(prefabCannonball, ship.position, Quaternion.identity);
        cb.GetComponent<Cannonball>().Move(ship);
        yield return new WaitForSeconds(Vector2.Distance(transform.position, ship.position) / 10);

        DestroyTriangle();
        triangleIndex++;
        LoadTriangle();
        onResumeControl?.Invoke();
    }

    public void PlaceCheckpoint(Transform checkpoint)
    {
        if (lr.positionCount >= 3) return;
        AfterPlace(checkpoint.position);

        if (lr.positionCount != 3) return;
        AfterPlace(transform.position);
        onLastCheckpointChecked?.Invoke(lr);
        StartCoroutine(ResetTriangle());
    }
}

[System.Serializable]
public class Triangles
{
    public Triangle[] TriangleArr;
}

[System.Serializable]
public class Triangle
{
    public string Answer;
    public string Question;
    public Vector2[] Vectors;
    public float[] Angles;
}
