using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisController : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 21;
    [SerializeField] private float interval = 1.8f;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private float moveDelta = 0.1f;

    TetrisContainer container;

    // runtime variables
    private float scale = 0.9f;
    private float timer = 0.0f;
    private TetrisBlock currentBlock;
    private GameObject[,] blocks;
    private float halfHeight;
    private float toWorldFactor = 1.0f;
    private bool isGameOver = false;

    // Start is called before the first frame update 
    void Start()
    {
        container = new TetrisContainer(width,height);
        transform.position = new Vector3(0, 0, 0);

        Camera mainCamera = Camera.main;
        float fov = mainCamera.fieldOfView;
        float aspect = mainCamera.aspect;
        if (mainCamera.orthographic)
        {
            halfHeight = mainCamera.orthographicSize;
        }
        else
        {
            halfHeight = Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad) * Mathf.Abs(mainCamera.transform.position.z);
        }
        float halfWidth = halfHeight * aspect;
        Debug.Log("halfHeight: " + halfHeight + " halfWidth: " + halfWidth);
        toWorldFactor = halfHeight * 2 / height;

        timer = interval;
        CreateBlock();

        if (blockPrefab == null) {
            Debug.LogError("预制体引用为空，请在编辑器中赋值。");
        }

        blocks = new GameObject[height, width];
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++)
            {
                blocks[i, j] = Instantiate(blockPrefab, 
                    new Vector3(j - width*0.5f, i - height*0.5f +0.5f, 0) * toWorldFactor, 
                    Quaternion.identity);
                blocks[i, j].transform.SetParent(this.transform);
                blocks[i, j].transform.localScale = new Vector3(scale, scale, scale) * toWorldFactor;
            }
        }
    }

    private float timerLeft = 0.0f;
    private float timerRight = 0.0f;
    private float timerDown = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) {
            return;
        }

        HandleInput();
    }

    void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0.0f) {
            timer += interval;
            MoveDown();
        }
    }

    void HandleInput() 
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            timerLeft = moveDelta;
            MoveLeft();
        } else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            timerLeft -= Time.deltaTime;
            if (timerLeft <= 0.0f)
            {
                timerLeft += moveDelta;
                MoveLeft();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            timerRight = moveDelta;
            MoveRight();
        } else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            timerRight -= Time.deltaTime;
            if (timerRight <= 0.0f) {
                timerRight += moveDelta;
                MoveRight();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            timerDown = moveDelta;
            MoveDown();
        } else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
            timerDown -= Time.deltaTime;
            if (timerDown <= 0.0f) {
                timerDown += moveDelta;
                MoveDown();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            HideBlock();
            currentBlock.RotateClockwise();
            if (!container.IsValid(currentBlock)) {
                currentBlock.RotateCounterClockwise();
            }
            ShowBlock();
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            HideBlock();
            while (true) {
                currentBlock.MoveDown();
                if (!container.IsValid(currentBlock)) {
                    currentBlock.MoveUp();
                    FallBlock();
                    break;
                }
            }
            ShowBlock();
        }
    }

    void MoveLeft()
    {
        HideBlock();
        currentBlock.MoveLeft();
        if (!container.IsValid(currentBlock)) {
            currentBlock.MoveRight();
        }
        ShowBlock();
    }

    void MoveRight()
    {
        HideBlock();
        currentBlock.MoveRight();
        if (!container.IsValid(currentBlock)) {
            currentBlock.MoveLeft();
        }
        ShowBlock();
    }

    void MoveDown()
    {
        HideBlock();
        currentBlock.MoveDown();
        if (!container.IsValid(currentBlock)) {
            currentBlock.MoveUp();
            FallBlock();
        }
        ShowBlock();
    }

    void FallBlock()
    {
        if (container.FallBlock(currentBlock)) {
            ShowBlock();
            (int beg, int end) = container.ClearRows(container.GetFullRows());
            UpdateContainer(beg, end);
            CreateBlock();
        } else {
            GameOver();
        }
    }

    void UpdateContainer(int beg, int end)
    {
        for (int i = beg; i < end; i++) {
            for (int j = 0; j < width; j++)
            {
                if (blocks[i, j] == null) {
                    continue;
                }
                bool hasValue = container.HasValue(i, j);
                if (hasValue) {
                    blocks[i, j].GetComponent<Renderer>().material.color = Color.red;
                } else {
                    blocks[i, j].GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over");
    }


    void ShowBlock()
    {
        Vector3 pos = currentBlock.GetPosition();
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(pos.y + block[i].y);
            if (y >= height) {
                continue;
            }
            int x = (int)(pos.x + block[i].x);
            blocks[y, x].GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void HideBlock()
    {
        Vector3 pos = currentBlock.GetPosition();
        Vector2[] block = currentBlock.GetBlock();
        for (int i = 0; i < block.Length; i++)
        {
            int y = (int)(pos.y + block[i].y);
            if (y >= height) {
                continue;
            }
            int x = (int)(pos.x + block[i].x);
            blocks[y, x].GetComponent<Renderer>().material.color = Color.white;
        }
    }

    void CreateBlock()
    {
        currentBlock = new TetrisBlock(new Vector3((int)(width * 0.5f + 0.5f), height - 1, 0));
    }
}
