using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    private Transform playerTr;

    public TileBase[] tileVarients;

    public int chunkSize = 32;
    public int seed = 12345;

    private Vector2Int currentChunk;

    void Reset()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTr = FindAnyObjectByType<Player>().transform;
        if(playerTr == null)
        {
            enabled = false;
            return;
        }

        tilemap ??= gameObject.GetComponent<Tilemap>();

        //시작할 때 청크 그려주기
        currentChunk = WorldToChunk(playerTr.position);
        DrawSurroundingChunks(currentChunk);
        //
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTr == null)
            return;

        Vector2Int nowChunk = WorldToChunk(playerTr.position);
        if(nowChunk != currentChunk)
        {
            currentChunk = nowChunk;
            DrawSurroundingChunks(currentChunk);
        }
    }

    private Vector2Int WorldToChunk(Vector2 worldPos)
    {
        int cX = Mathf.FloorToInt(worldPos.x / chunkSize);
        int cY = Mathf.FloorToInt(worldPos.y / chunkSize);
        return new Vector2Int(cX, cY);
    }

    private void DrawSurroundingChunks(Vector2Int center)
    {
        tilemap.ClearAllTiles();

        for(int cY = -1; cY <= 1; cY++)
        {
            for (int cX = -1; cX <= 1; cX++)
            {
                var c = new Vector2Int(center.x + cX, center.y + cY);
                DrawChunk(c);
            }
        }
    }

    private void DrawChunk(Vector2Int c)
    {
        int startX = c.x * chunkSize;
        int startY = c.y * chunkSize;

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                int tX = startX + x;
                int tY = startY + y;

                // 그리는 함수
                tilemap.SetTile(new Vector3Int(tX, tY, 0), PickRandomTile(tX, tY));
            }
        }
    }

    private TileBase PickRandomTile(int x, int y)
    {
        if (tileVarients == null || tileVarients.Length <= 0)
            return null;

        int hash = (x * 73856093) ^ (y * 19349663) ^ seed;
        if (hash < 0) hash *= -1;

        int idx = hash % tileVarients.Length;
        return tileVarients[idx];
    }

}
