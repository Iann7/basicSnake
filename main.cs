using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class main : MonoBehaviour
{
    // 0 IS FOR BLACK ; 1 IS FOR WHITE ; 2 IS FOR GREEN ; 3 IS FOR RED 
    public Tile black, white, green, red;
    public Tilemap tilemap;

    public int maxX, minX, maxY, minY;

    public float totalTime;
    private float currentTime;

    public GameObject DeadNotification;
    private bool isSnakeDead;
    

    private Vector3Int snakeDirection;
    private List<Vector3Int> snakePosition;
    private List<Vector3Int> emptyPositions;
    private Vector3Int applePosition;
    // Start is called before the first frame update
    void Start()
    {
        isSnakeDead = false;

        currentTime = totalTime;
        DeadNotification.SetActive(false);

        snakeDirection = new Vector3Int(0, 0, 0);
        snakePosition = new List<Vector3Int>();

        GenerateApplePosition();

        snakePosition.Add(new Vector3Int(10, 10, 0));
        snakePosition.Add(new Vector3Int(10, 9, 0));
        snakePosition.Add(new Vector3Int(10, 8, 0));

        CreateMapBounds();

        foreach (Vector3Int tempVec in snakePosition)
        {
            tilemap.SetTile(tempVec, green);
        }
    }

    void CreateMapBounds()
    {
        //DrawLine(maxX,minX,white);
        DrawHorizontalLine(maxX,minX,minY,white);
        DrawHorizontalLine(maxX, minX, maxY, white);

        DrawVerticalLine(maxY, minY, minX, white);
        DrawVerticalLine(maxY, minY, maxX, white);
    }

    void GenerateApplePosition()
    {
        //MAKE CHECKS WITH SNAKE POS 
        int x, y;
        x = Random.Range(minX+1, maxX-1);
        y = Random.Range(minY+1, maxY-1);
        applePosition = new Vector3Int(x, y, 0);
        if (snakePosition.Contains(applePosition)) GenerateApplePosition();
        tilemap.SetTile(applePosition, red);
    }

    void DrawHorizontalLine(int end,int start,int altitude,Tile tile)
    {
        for(int s = start; s <= end; s++)
        {
            tilemap.SetTile(new Vector3Int(s, altitude, 0), tile);
        }
    }

    void DrawVerticalLine(int end, int start, int altitude, Tile tile)
    {
        for (int s = start; s <= end; s++)
        {
            tilemap.SetTile(new Vector3Int(altitude,s, 0), tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangingSnakeDirection();
       
        currentTime -= Time.deltaTime;
        //Debug.Log("CURRENT TIME" + currentTime);
        if (currentTime <= 0 && !isSnakeDead)
        {
            Debug.Log("INSIDE UPDATE");
            currentTime = totalTime;
            MoveSnake();
        }
    }

    void ChangingSnakeDirection()
    {

        // MAY NEED TO CHANGE POS CHECK 
        Vector3Int tempSnakeDirection = Vector3Int.zero;
        Vector3Int tempSnakePosition = snakePosition[snakePosition.Count - 1];
        if (Input.GetKeyDown(KeyCode.LeftArrow)) tempSnakeDirection = new Vector3Int(-1,0,0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) tempSnakeDirection = new Vector3Int(1, 0, 0);
        if (Input.GetKeyDown(KeyCode.UpArrow)) tempSnakeDirection = new Vector3Int(0, 1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) tempSnakeDirection = new Vector3Int(0, -1, 0);

        if (tempSnakeDirection == Vector3Int.zero) return;
        if (snakePosition.Contains(tempSnakeDirection + tempSnakePosition))
        {
            if (snakePosition.Count == 1) return;
            else
            {
                if (snakePosition[snakePosition.Count - 2] == (tempSnakeDirection + tempSnakePosition)) return;
                DeadSnake();
            }
        }
        else snakeDirection = tempSnakeDirection;

       
    }

    void MoveSnake()
    {
        Vector3Int newSnakePos = snakePosition[snakePosition.Count - 1] + snakeDirection;
        if (!IsInsideBounds(newSnakePos))
        {
            Debug.Log("DEAD SNAKE");
            DeadSnake();
            return;
        }

        if (!hasSnakeChomped(newSnakePos)){
            tilemap.SetTile(snakePosition[0], black);
            snakePosition.RemoveAt(0);
        }

        snakePosition.Add(newSnakePos);
        tilemap.SetTile(newSnakePos, green);
    }

    bool hasSnakeChomped(Vector3Int tempSnakePos)
    {
        if (tempSnakePos == applePosition)
        {
            GenerateApplePosition();
            return true;
        }
        return false;
    }

    bool IsInsideBounds(Vector3Int tempSnakePos)
    {
        if (tempSnakePos.x == maxX || tempSnakePos.x == minX) return false;
        if (tempSnakePos.y == maxY || tempSnakePos.y == minY) return false;
        return true;
    }

    void DeadSnake()
    {
        isSnakeDead = true;
        DeadNotification.gameObject.SetActive(true);
    }
}
