using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Centre : MonoBehaviour
{
    public static Centre instance;

    static readonly float r6 = Mathf.Sqrt(6);
    static readonly float r3 = Mathf.Sqrt(3);

    static readonly float top = r6 / 2;
    static readonly float middle = r6 / 4;

    const int faceCount = 8;
    List<int>[] blocksInFace = new List<int>[faceCount];
    public enum FACE { U, u, F, f, L, l, R, r };

    Octahedron[] octa = new Octahedron[4];
    public Tetrahedron[] tetra = new Tetrahedron[15];

    Vector3[] pivot = new Vector3[4]
    {
        new(0, 1, 0),
        new Vector3(0, -r6 / 12, -r3 / 3).normalized,
        new Vector3(-0.5f, -r6 / 12, r3 / 6).normalized,
        new Vector3(0.5f, -r6 / 12, r3 / 6).normalized
    };

    [Header("Prefabs")]
    public Tetrahedron tetraPrefab;
    public Octahedron octaPrefab;

    float _Fast = 0.1f;
    float _Medium = 0.15f;
    float _Slow = 0.2f;

    int slowBound = 2;
    int mediumBound = 5;

    bool isRotating = false;
    bool isFlipped = false;
    bool isSingle = false;

    Queue<Rotation> rotationQueue = new();

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;

            InitiateData();
        } else {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            isFlipped = true;
        } else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
            isFlipped = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) {
            isSingle = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) {
            isSingle = false;
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            AddToQueue(pivot[0], !isSingle ? FACE.u : FACE.U, isFlipped);
        }
        else if (Input.GetKeyDown(KeyCode.F)) {
            AddToQueue(pivot[1], !isSingle ? FACE.f : FACE.F, isFlipped);
        }
        else if (Input.GetKeyDown(KeyCode.L)) {
            AddToQueue(pivot[2], !isSingle ? FACE.l : FACE.L, isFlipped);
        }
        else if (Input.GetKeyDown(KeyCode.R)) {
            AddToQueue(pivot[3], !isSingle ? FACE.r : FACE.R, isFlipped);
        }

        Rotate();
    }

    private void AddToQueue(Vector3 axis, FACE face, bool flipped)
    {
        axis *= flipped ? -1 : 1;
        int count = rotationQueue.Count;
        float duration = count <= slowBound ? _Slow : count <= mediumBound ? _Medium : _Fast;
        rotationQueue.Enqueue(new(axis, duration, face, flipped));
    }

    private void Rotate()
    {
        if (!isRotating && rotationQueue.Count > 0) {
            StartCoroutine(StartRotation(rotationQueue.Dequeue()));
        }
    }

    private IEnumerator StartRotation(Rotation rotation)
    {
        isRotating = true;
        Tie(rotation.face, rotation.reversed);

        float percentage = 0;
        Quaternion startRotation = gameObject.transform.rotation;
        Quaternion endRotation = Quaternion.AngleAxis(120f, rotation.axis);
        while (percentage < 1.0f) {
            percentage += Time.deltaTime / rotation.duration;
            gameObject.transform.rotation = Quaternion.Slerp(startRotation, endRotation, percentage);
            yield return null;
        }
        gameObject.transform.rotation = endRotation;

        Untie(rotation.face);
        isRotating = false;
    }

    private void Tie(FACE face, bool reversed)
    {
        if ((int)face % 2 != 0) octa[(int)face / 2].gameObject.transform.parent = gameObject.transform;
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var block = tetra[blocksInFace[(int)face][i]];
            block.gameObject.transform.parent = gameObject.transform;
            block.ChangePos((int)face / 2, reversed);
        }
    }

    private void Untie(FACE face)
    {
        if ((int)face % 2 != 0) octa[(int)face / 2].gameObject.transform.parent = null;
        List<Tetrahedron> blocks = new();
        for (int i = 0; i < blocksInFace[(int)face].Count; ++i) {
            var block = tetra[blocksInFace[(int)face][i]];
            block.gameObject.transform.parent = null;
            blocks.Add(block);
        }
        for (int i = 0; i < blocks.Count; ++i) {
            blocks[i].Relocate();
        }
        gameObject.transform.rotation = Quaternion.identity;
    }

    private void InitiateData()
    {
        for (int i = 0; i < 4; ++i) {
            octa[i] = Instantiate(octaPrefab);
            octa[i].transform.position = pivot[i] * middle;
        }

        for (int i = 0; i < faceCount; i++) {
            blocksInFace[i] = new List<int>();
        }

        for (int i = 0; i < 4; ++i) {
            blocksInFace[i * 2].Add(1 << i);
            blocksInFace[i * 2 + 1].Add(1 << i);
            tetra[1 << i] = Instantiate(tetraPrefab);
            tetra[1 << i].pos = 1 << i;
            tetra[1 << i].transform.position = pivot[i] * top;
        }

        for (int i = 0; i < 4; ++i) {
            for (int j = i + 1; j < 4; ++j) {
                blocksInFace[i * 2 + 1].Add((1 << i) + (1 << j));
                blocksInFace[j * 2 + 1].Add((1 << i) + (1 << j));
                tetra[(1 << i) + (1 << j)] = Instantiate(tetraPrefab);
                tetra[(1 << i) + (1 << j)].pos = (1 << i) + (1 << j);
                tetra[(1 << i) + (1 << j)].transform.position =
                    (tetra[1 << i].transform.position + tetra[1 << j].transform.position) / 2;
            }
        }

        MeshRenderer mr;
        
        mr = octa[0].transform.Find("Left").GetComponent<MeshRenderer>();
        mr.material.color = Color.blue;
        mr = octa[0].transform.Find("Right").GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
        mr = octa[0].transform.Find("Back").GetComponent<MeshRenderer>();
        mr.material.color = Color.green;

        mr = octa[1].transform.Find("Left").GetComponent<MeshRenderer>();
        mr.material.color = Color.blue;
        mr = octa[1].transform.Find("Right").GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
        mr = octa[1].transform.Find("Buttom").GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;

        mr = octa[2].transform.Find("Left").GetComponent<MeshRenderer>();
        mr.material.color = Color.blue;
        mr = octa[2].transform.Find("Buttom").GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;
        mr = octa[2].transform.Find("Back").GetComponent<MeshRenderer>();
        mr.material.color = Color.green;

        mr = octa[3].transform.Find("Buttom").GetComponent<MeshRenderer>();
        mr.material.color = Color.yellow;
        mr = octa[3].transform.Find("Right").GetComponent<MeshRenderer>();
        mr.material.color = Color.red;
        mr = octa[3].transform.Find("Back").GetComponent<MeshRenderer>();
        mr.material.color = Color.green;

        Color[] colors = new Color[4] {
            Color.yellow,
            Color.green,
            Color.red,
            Color.blue,
        };

        string[] names = new string[4]
        {
            "Buttom",
            "Back",
            "Right",
            "Left"
        };

        for (int i = 0; i < 4; ++i) {
            for (int j = 1; j < 13; ++j) {
                if (tetra[j] == null) continue;
                if ((j >> i & 1) == 0) {
                    mr = tetra[j].transform.Find(names[i]).GetComponent<MeshRenderer>();
                    mr.material.color = colors[i];
                }
            }
        }
    }
}
