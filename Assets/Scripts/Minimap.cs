using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    private GameObject[,] mapTiles; // should show walkable tiles

    [SerializeField] GameObject mapTilePrefab;
    [SerializeField] GameObject playerIconPrefab;

    [SerializeField] GameObject playerIcon;

    [SerializeField] float mapScaleDist = 10;

    [SerializeField] Vector2 playerMapPos;

    private void Awake()
    {
        playerMapPos = new Vector2(transform.position.x, transform.position.y);
    }

    private void Update()
    {
        playerMapPos = GetPlayerPosition();

        //Debug.Log(new Vector2(transform.position.x + (playerMapPos.x * 10), transform.position.y + (playerMapPos.y * 10)));

        if(playerIcon != null)
        {
            playerIcon.transform.position = new Vector2(transform.position.x + (playerMapPos.x * mapScaleDist), transform.position.y + (playerMapPos.y * mapScaleDist));
        }
    }

    private void DrawPlayerIcon()
    {
        if(playerIcon != null)
        {
            Destroy(playerIcon);
        }
        playerIcon = Instantiate(playerIconPrefab, transform);
        playerIcon.transform.position = playerMapPos;
    }

    public void DestroyMinimap()
    {
        if (mapTiles != null)
        {
            //Debug.Log("Destroying minimap.");
            for (int x = 0; x < mapTiles.GetLength(0); x++)
            {
                for (int y = 0; y < mapTiles.GetLength(1); y++)
                {
                    //Debug.Log(mapTiles[x, y].name);
                    Destroy(mapTiles[x, y]);
                }
            }
        }

    }

    public void CreateMap(bool[,] walkableMap)
    {
        //Debug.Log("Drawing minimap.");
        DestroyMinimap();
        mapTiles = new GameObject[walkableMap.GetLength(0), walkableMap.GetLength(1)];
        for (int x = 0; x < walkableMap.GetLength(0); x++)
        {
            for (int y = 0; y < walkableMap.GetLength(1); y++)
            {
                GameObject newMapTile = Instantiate(mapTilePrefab,transform);
                newMapTile.transform.position = new Vector2(transform.position.x + (x * mapScaleDist), transform.position.y + (y * mapScaleDist));
                mapTiles[x, y] = newMapTile;
                if(walkableMap[x,y] != true)
                {
                    newMapTile.GetComponent<Image>().enabled = false;
                }
            }
        }
        DrawPlayerIcon();
    }

    public Vector2 GetPlayerPosition()
    {
        OWPlayerController owPlayer = GameObject.Find("OWPlayer").GetComponent<OWPlayerController>();
        return new Vector2(owPlayer.transform.position.x, owPlayer.transform.position.z);
    }
}
