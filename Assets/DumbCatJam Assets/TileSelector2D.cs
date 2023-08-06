using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerMovement))]


public class TileSelector2D : MonoBehaviour
{
    enum TileMode
    {
        Destroy,
        Spike,
        Wall,
        Bounce, 
        None
    }
    public Transform crosshair;
    public PlayerMovement playerMovement;
    [SerializeField] Transform playerCameraFollow;
    [SerializeField] Transform lookAtObject;
    public float cameraOffsetDistance = .35f;
    [SerializeField] float camTargetSpeed = .25f;
    internal Vector3 aimDirection;
    internal float aimDistance = 1.0f;
    [SerializeField] TileMode m_TileMode = TileMode.None;

    [SerializeField] Tilemap tileMap, spikeTileMap, wallTileMap, bounceTileMap, backgroundTileMap;
    [SerializeField] TileBase spikeTile, wallTile, bounceTile;

    Vector2 worldPoint;
    float mouseDistance;

    private void Start()
    {
        playerCameraFollow.transform.parent = null;
    }

    private void Awake() {
        playerMovement = GetComponent<PlayerMovement>();
        tileMap = wallTileMap;
    }
    void OnPosition(InputValue value)
    {
        if (!playerMovement.GetIsAlive()) { return; }
        worldPoint = Camera.current.ScreenToWorldPoint(value.Get<Vector2>());

        Debug.Log("Position: " + value.Get<Vector2>());
    }
    
    void OnScrollWheel()
    {
        //increment or decrement tile mode when scrolling
        
        int mode = (int)m_TileMode;
        mode++;
        if(mode > 3)
        {
            mode = 0;
        }
    }

    void OnFire(InputValue value)
    {
        if (!playerMovement.GetIsAlive()) { return; }

        
        // var tpos = tileMap.WorldToCell(worldPoint);

        // // Try to get a tile from cell position
        // var tile = tileMap.GetTile(tpos);         

        // Debug.Log("Clicking Tile: " + tile.name + " at " + tpos + "with world point " + worldPoint);
        
        switch(m_TileMode)
        {
            case TileMode.Destroy:
                DestroyTile();
                break;
            case TileMode.Bounce:
                ReplaceWithTile(bounceTileMap, bounceTile);
                break;
            case TileMode.Spike:
                ReplaceWithTile(spikeTileMap, spikeTile);
                break;
            case TileMode.Wall:
                ReplaceWithTile(wallTileMap, wallTile);
                break;
        }

        // if(tile)
        // {
        //     Debug.Log("Clicking Tile: " + tile.name + " at " + tpos + "with world point " + worldPoint);
        //     switch(m_TileMode)
        //     {
        //         case TileMode.Destroy:
        //             DestroyTile();
        //             break;
        //         case TileMode.Bounce:
        //             ReplaceWithTile(bounceTileMap, bounceTile);
        //             break;
        //         case TileMode.Spike:
        //             ReplaceWithTile(spikeTileMap, spikeTile);
        //             break;
        //         case TileMode.Wall:
        //             ReplaceWithTile(wallTileMap, wallTile);
        //             break;
        //     }
        // }
        // else
        // {
        //     if(m_TileMode == TileMode.Wall)
        //     {
        //         ReplaceWithTile(wallTileMap, wallTile);
        //     }
            
        // }
    }

    // Update is called once per frame
    void Update()
    {
        worldPoint = MoveMouseCursor();
        // worldPoint = Camera.main.ScreenToWorldPoint();            
        var tpos = tileMap.WorldToCell(worldPoint);
        // Try to get a tile from cell position
        var tile = tileMap.GetTile(tpos);          
    }

    Vector3 MoveMouseCursor()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        crosshair.transform.position = mouseWorldPos;

        return mouseWorldPos;
    }

    void MouseAimOther()
    {
        aimDirection = Vector3.zero;
        float aimAngle = 0;
        // var worldMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        mouseDistance = Vector2.Distance(worldPoint, transform.position);
        var facingDirection = (Vector3)worldPoint - transform.position;
        aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        aimDirection = Quaternion.Euler(0, 0 , aimAngle * Mathf.Rad2Deg) * Vector2.right;
        SetCrosshairPosition(aimAngle);
        playerCameraFollow.transform.position = transform.position;
        lookAtObject.localPosition = Vector3.Lerp(lookAtObject.localPosition, aimDirection * cameraOffsetDistance, Time.deltaTime * camTargetSpeed);
    }

    public void DestroyTile()
    {
        //Destroy the tile at the position on all tilemaps
        var tpos = tileMap.WorldToCell(worldPoint);
        tileMap.SetTile(tpos, null);

        tpos = wallTileMap.WorldToCell(worldPoint);
        wallTileMap.SetTile(tpos, null);

        tpos = spikeTileMap.WorldToCell(worldPoint);
        spikeTileMap.SetTile(tpos, null);
        
        tpos = bounceTileMap.WorldToCell(worldPoint);
        bounceTileMap.SetTile(tpos, null);
    }

    public void SetTileMode(int mode)
    {
        m_TileMode = (TileMode)mode;
    }

    void ReplaceWithTile(Tilemap _tileMap, TileBase tile)
    {
        // var prevTileMap = get
        DestroyTile();
        var tpos = _tileMap.WorldToCell(worldPoint);
        _tileMap.SetTile(tpos, tile);

        // tileMap.SetTile(tpos, null);                
        // tpos = _tilemap.WorldToCell(worldPoint);        
        //if a tile from another tilemap is at the position, destroy it
       
        //Replace with new tile
    }


    /// <summary>
    /// Move the aiming crosshair based on aim angle
    /// </summary>
    /// <param name="aimAngle">The mouse aiming angle</param>
    public void SetCrosshairPosition(float aimAngle)
    {

        if (mouseDistance > aimDistance)
        {
            mouseDistance = aimDistance;
        }

        var x = transform.position.x + 1f * Mathf.Cos(aimAngle) * mouseDistance;
        var y = transform.position.y + 1f * Mathf.Sin(aimAngle) * mouseDistance;

        var crossHairPosition = new Vector3(x, y, 0);
        crosshair.transform.position = crossHairPosition;
    }
}
