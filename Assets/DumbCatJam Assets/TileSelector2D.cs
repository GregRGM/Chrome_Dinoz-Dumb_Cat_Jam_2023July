using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;


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
    [SerializeField] int m_TileChargesRemaining = 0, m_TileChargesMax = 20;
    TextMeshProUGUI tileModeText, tileChargesText;
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

        if(tileModeText == null)
            tileModeText = GameObject.Find("Tile Mode Text").GetComponent<TextMeshProUGUI>();
        tileModeText.text = "Tile Mode: " + m_TileMode.ToString();        

        m_TileChargesRemaining = m_TileChargesMax;
        m_TileChargesRemaining = Mathf.Clamp(m_TileChargesRemaining, 0, m_TileChargesMax);
        
        if(tileChargesText == null)
            tileChargesText = GameObject.Find("Tile Charges Text").GetComponent<TextMeshProUGUI>();
        tileChargesText.text = "Tile Switches: " + m_TileChargesRemaining.ToString();
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
        if (!playerMovement.GetIsAlive() ) { return; }

        
        // var tpos = tileMap.WorldToCell(worldPoint);

        // // Try to get a tile from cell position
        // var tile = tileMap.GetTile(tpos);         

        // Debug.Log("Clicking Tile: " + tile.name + " at " + tpos + "with world point " + worldPoint);
        if(m_TileChargesRemaining <= 0)
        {
            Debug.Log("No more tile switches remaining");
            return;
        }

        switch(m_TileMode)
        {
            case TileMode.Destroy:
                DestroyTile();
                break;
            case TileMode.Bounce:
                ReplaceWithTile(bounceTileMap, bounceTile);
                m_TileChargesRemaining--;
                break;
            case TileMode.Spike:
                ReplaceWithTile(spikeTileMap, spikeTile);
                m_TileChargesRemaining--;
                break;
            case TileMode.Wall:
                ReplaceWithTile(wallTileMap, wallTile);
                m_TileChargesRemaining--;
                break;
        }
        tileChargesText.text = "Tile Switches: " + m_TileChargesRemaining.ToString();
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


        // if(Input.GetAxis("Mouse ScrollWheel") != 0)
        // {
        //     int mode = (int)m_TileMode;
        //     mode++;
        //     if(mode > 3)
        //     {
        //         mode = 0;
        //     }
        //     m_TileMode = (TileMode)mode;
            
        // }
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            int mode = (int)m_TileMode;
            mode--;
            if(mode < 0)
            {
                mode = 3;
            }
            m_TileMode = (TileMode)mode;
            tileModeText.text = m_TileMode.ToString();            
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            int mode = (int)m_TileMode;
            mode++;
            if(mode > 3)
            {
                mode = 0;
            }
            m_TileMode = (TileMode)mode;
            tileModeText.text = m_TileMode.ToString();
        }
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

        //Replenish the tiles remaining
        //tilesRemaining++;
    }

    public void SetTileMode(int mode)
    {
        m_TileMode = (TileMode)mode;
    }

    void ReplaceWithTile(Tilemap _tileMap, TileBase tile)
    {
        //Check if a player or enemy is in the tile position, return if true
        
        

        // var prevTileMap = get
        DestroyTile();
        var tpos = _tileMap.WorldToCell(worldPoint);
        //Get the current layer for player and enemy
        var playerLayer = LayerMask.GetMask("Player", "Enemy");
        if(Physics2D.OverlapCircle(worldPoint, 0.5f, playerLayer) != null)
        {
            return;
        }
        _tileMap.SetTile(tpos, tile);

        // tileMap.SetTile(tpos, null);                
        // tpos = _tilemap.WorldToCell(worldPoint);        
        //if a tile from another tilemap is at the position, destroy it
       
        //Replace with new tile
    }

    public void AddToCharges(int charges)
    {
        m_TileChargesRemaining += charges;
        m_TileChargesRemaining = Mathf.Clamp(m_TileChargesRemaining, 0, m_TileChargesMax);
        tileChargesText.text = "Tile Switches: " + m_TileChargesRemaining.ToString();
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
