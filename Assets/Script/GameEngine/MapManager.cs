using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class MapManager : SingletonComponent<MapManager>
{

    #region Unity Functions
    protected override void Awake()
    {
        base.Awake();


        AddConstructableGrid();

        gridLayout = transform.parent.GetComponentInParent<GridLayout>();
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    void OnEnable()
    {
        EventManager.Instance.OnPlayerLevelUp += OnPlayerLevelUp;
    }

    void OnDisable()
    {

        if (EventManager.Instance)
        {
            EventManager.Instance.OnPlayerLevelUp -= OnPlayerLevelUp;
        }
    }

    void Start()
    {
        if(Instance.secondCalled)
        {
            SetExpandedArea();

        }
        
    }

    void Update()
    {
        // Debug.Log(tilemap.localBounds.ToString());
        if (buildPermission && Input.GetMouseButtonUp(0))
        {

            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector3 mouseToWorldPoint = Camera.main.ScreenToWorldPoint(mousePos);
            mouseToWorldPoint.z = 0;

            RaycastHit2D rayHit = Physics2D.Raycast(mouseToWorldPoint, Vector2.zero);
            if (rayHit.collider != null && rayHit.collider.gameObject.tag == "Tree")
            {
                return;
            }

            //  Debug.Log("world : " + mouseToWorldPoint.ToString());
            Vector3Int clickedCellPosition = gridLayout.WorldToCell(mouseToWorldPoint);
            //  Debug.Log("cell " + clickedCellPosition.ToString());
            ClickTile(clickedCellPosition);

        }

    }
    #endregion

    Tilemap tilemap;
    public GridLayout gridLayout;

    public Tile selectedTile;
    public Tile defaultTileMap;
    Dictionary<int, List<Vector3Int[]>> constructableGridDictionary;
    public List<Vector3Int[]> constructableGrid;

    int BUILDING_POSITION_OFFSET = 1;

    bool buildPermission;

    /// <summary>
    /// Destroy expanded area(Trees) and open FogArea.
    /// </summary>
    public void SetExpandedArea()
    {
        Debug.Log("Seting expanded Area.");
        foreach (int treeID in LoadManager.Instance.playerData.expandedArea)
        {
            GameObject treeGO = GameManager.FindDeepChild(GameObject.Find("Map/Trees").transform, treeID.ToString()).gameObject;

            if (treeGO)
            {
                Destroy(treeGO);
            }
            
        }

        for (int i = 2; i <= LoadManager.Instance.playerData.level / 5; i++)
        {
            GameObject fogGO = GameObject.Find("Map/FogArea/" + i);
            if (fogGO)
            {
                Destroy(fogGO);
            }
        }

        return;
    }
    void OnPlayerLevelUp(int level)
    {
        if (level % 5 == 0)
        {
            GameObject fogGO = GameObject.Find("Map/FogArea/" + level / 5);
            if (fogGO)
            {
                Destroy(fogGO);

            }

        }

    }
    public string SelectedBuildingName { get; set; }

    /// <summary>
    /// Select where to Create Building.
    /// </summary>
    /// <param name="position"> Building Position </param>
    public void ClickTile(Vector3Int position)
    {
        for (int i = 0; i < constructableGrid.Count; i++)
        {
            if (constructableGrid[i].Contains(position))
            {
                /// Prevent from immediately showing Building Information Panel after build. 
                Resources.FindObjectsOfTypeAll<BuildingShopPanel>()[0].gameObject.SetActive(false);
                MainCanvas.canvasActive = true;
                StartCoroutine(DelaySetCanvasActive(false));
                /// -------------------------------------------------------------------------------
                Vector2 adjustedPosition = CalculateBuildPosition(i);
                 Action<int> callback = (teamNumber) =>
                {
                    TeamSelectorCallback((Building.BuildingType)int.Parse(SelectedBuildingName),
                    teamNumber, adjustedPosition, i);

                };

                Builder builder = new Builder((Building.BuildingType)int.Parse(SelectedBuildingName));
                ShowTeamSelectorPanel(builder, callback);

                return;
            }
        }
    }
    public Vector2 CalculateBuildPosition(int cellIndex)
    {
        int maxX = constructableGrid[cellIndex].Select(g1 => g1.x).Max();
        int maxY = constructableGrid[cellIndex].Select(g2 => g2.y).Max();

        Vector3Int currentCellPosition = constructableGrid[cellIndex].SingleOrDefault(g => g.x == maxX && g.y == maxY);
        return gridLayout.CellToWorld(new Vector3Int(currentCellPosition.x + BUILDING_POSITION_OFFSET, currentCellPosition.y + BUILDING_POSITION_OFFSET, 0));

    }

    public void LoadTile(Builder builder)
    {
        for (int i = 0; i < constructableGrid.Count; i++)
        {
            Vector3Int cell = gridLayout.WorldToCell(builder.Position);
            if (constructableGrid[i].Contains(new Vector3Int(cell.x - BUILDING_POSITION_OFFSET, cell.y - BUILDING_POSITION_OFFSET, 0)))
            {
                BuildManager.Instance.LoadBuilding(builder);
                constructableGrid.RemoveAt(i);
                return;
            }
            /// Debug
            //Debug.Log($"{position} floors to {gridLayout.WorldToCell(position)} compares with {constructableGrid[i][0]},  {constructableGrid[i][1]},  {constructableGrid[i][2]},  {constructableGrid[i][3]}");
        }

        Debug.LogError($"LoadTile of {builder.Type} have some problems !");
        return;
    }

    public void ShowAvailableTiles()
    {
        buildPermission = true;

        Tile[] t = new Tile[16];
        for (int i = 0; i < t.Length; i++)
        {
            t[i] = selectedTile;
        }

        for (int i = 0; i < constructableGrid.Count; i++)
        {
            tilemap.SetTiles(constructableGrid[i], t);
        }

        tilemap.RefreshAllTiles();
    }

    public void CancleShowAvailableTiles()
    {
        buildPermission = false;
        //  Debug.Log("Clearing Available Tiles . . .");
        Tile[] t = new Tile[16];
        for (int i = 0; i < t.Length; i++)
        {
            t[i] = defaultTileMap;
        }
        for (int i = 0; i < constructableGrid.Count; i++)
        {
            tilemap.SetTiles(constructableGrid[i], t);
        }

        tilemap.RefreshAllTiles();
    }

    public bool PurchaseNewArea(int areaID, int purchaseOption) /// 0 = Gold, 1 = Diamond
    {
        GameObject treeGO = GameManager.FindDeepChild(GameObject.Find("Map/Trees").transform, areaID.ToString()).gameObject;
        int fogAreaID = int.Parse(treeGO.transform.parent.name.Replace("Group", ""));

        int expandPrice = fogAreaID * (purchaseOption == 0 ? 5 : 1);

        string resourceName = purchaseOption == 0 ? "Gold" : "Diamond";
        if (ItemManager.Instance.TryConsumeResources(resourceName, expandPrice))
        {
            ItemManager.Instance.AddResource("Wood", 10);
            LoadManager.Instance.playerData.expandedArea.Add(areaID);
            Destroy(GameManager.FindDeepChild(GameObject.Find("Map/Trees").transform, areaID.ToString()).gameObject);
            return true;
        }
        else
        {

            return false;
        }
    }

    void AddConstructableGrid()
    {
        constructableGridDictionary = new Dictionary<int, List<Vector3Int[]>>();

        constructableGrid = new List<Vector3Int[]>();
        for (int i = 0; i < 60; i++)
        {
            constructableGrid.Add(new Vector3Int[16]);

        }

        List<Vector3Int> startedConstructableGrid = new List<Vector3Int>()
        {
            new Vector3Int(16, 125, 0),
            new Vector3Int(32, 111, 0),
            new Vector3Int(16, 111, 0),
            new Vector3Int(1, 112, 0),
            new Vector3Int(-25, 86, 0),
            new Vector3Int(4, 84, 0),
            new Vector3Int(20, 84, 0),
            new Vector3Int(34, 85, 0),
            new Vector3Int(61, 83, 0),

            new Vector3Int(-48, 63, 0),
            new Vector3Int(-64, 45, 0),
            new Vector3Int(-48, 44, 0),

            new Vector3Int(-79, 23, 0),

            new Vector3Int(-81, 4, 0),
            new Vector3Int(-118, -25, 0),

            new Vector3Int(-100, -25, 0),

            new Vector3Int(-82, -27, 0),

            new Vector3Int(-101, -44, 0),

            new Vector3Int(-80, -49, 0),

            new Vector3Int(-70, -73, 0),

            new Vector3Int(-57, -83, 0),

            new Vector3Int(-43, -72, 0),

            new Vector3Int(-32, -83, 0),

            new Vector3Int(-16, -71, 0),

            new Vector3Int(-30, -113, 0),

            new Vector3Int(-8, -112, 0),

            new Vector3Int(-20, -125, 0),

            new Vector3Int(11, -92, 0),

            new Vector3Int(14, -74, 0),

            new Vector3Int(32, -73, 0),

            new Vector3Int(63, -41, 0),

            new Vector3Int(63, -26, 0),

            new Vector3Int(81, -25, 0),

            new Vector3Int(64, 54, 0),

            new Vector3Int(86, 51, 0),

            new Vector3Int(69, 34, 0),

            new Vector3Int(90, 34, 0),

            new Vector3Int(110, 31, 0),

            new Vector3Int(64, 14, 0),

            new Vector3Int(85, 9, 0),

            new Vector3Int(111, 10, 0),

            new Vector3Int(-23, 58, 0),

            new Vector3Int(-23, 44, 0),

            new Vector3Int(-48, 17, 0),

            new Vector3Int(-49, 3, 0),

            new Vector3Int(-32, -27, 0),

            new Vector3Int(-46, -44, 0),

            new Vector3Int(-20, -41, 0),

            new Vector3Int(9, -25, 0),

            new Vector3Int(-99, 9, 0),

            new Vector3Int(31, -28, 0),

            new Vector3Int(36, -45, 0),

            new Vector3Int(5, 54, 0),

            new Vector3Int(17, 43, 0),
            new Vector3Int(33, 56, 0),

            new Vector3Int(-23, 13, 0),

            new Vector3Int(-6, 6, 0),

            new Vector3Int(17, 15, 0),
            new Vector3Int(34, 5, 0),

            new Vector3Int(14, -43, 0)
      
    };


        AddconstructableGrid(startedConstructableGrid);

      
    }

    void AddconstructableGrid(List<Vector3Int> startedpoint)
    {
        int x = 0;


        for (int i = 0; i < startedpoint.Count; i++)
        {

            constructableGrid[x][0] = startedpoint[i];
            constructableGrid[x][1] = startedpoint[i] + new Vector3Int(0, -1, 0);
            constructableGrid[x][2] = startedpoint[i] + new Vector3Int(0, -2, 0);
            constructableGrid[x][3] = startedpoint[i] + new Vector3Int(0, -3, 0);

            constructableGrid[x][4] = startedpoint[i] + new Vector3Int(+1, 0, 0);
            constructableGrid[x][5] = startedpoint[i] + new Vector3Int(+1, -1, 0);
            constructableGrid[x][6] = startedpoint[i] + new Vector3Int(+1, -2, 0);
            constructableGrid[x][7] = startedpoint[i] + new Vector3Int(+1, -3, 0);

            constructableGrid[x][8] = startedpoint[i] + new Vector3Int(+2, 0, 0);
            constructableGrid[x][9] = startedpoint[i] + new Vector3Int(+2, -1, 0);
            constructableGrid[x][10] = startedpoint[i] + new Vector3Int(+2, -2, 0);
            constructableGrid[x][11] = startedpoint[i] + new Vector3Int(+2, -3, 0);

            constructableGrid[x][12] = startedpoint[i] + new Vector3Int(+3, 0, 0);
            constructableGrid[x][13] = startedpoint[i] + new Vector3Int(+3, -1, 0);
            constructableGrid[x][14] = startedpoint[i] + new Vector3Int(+3, -2, 0);
            constructableGrid[x][15] = startedpoint[i] + new Vector3Int(+3, -3, 0);
            x++;
        }
    }

    void ShowTeamSelectorPanel(Builder builder, Action<int> callback)
    {
        Builder laborCenter = BuildManager.Instance.AllBuildings.SingleOrDefault(b => b.Type == Building.BuildingType.LaborCenter);

        if (laborCenter != null && laborCenter.Level != 0)
        {
            TeamSelectorPanel teamSelectorPanel = Resources.FindObjectsOfTypeAll<TeamSelectorPanel>()[0];
            teamSelectorPanel.gameObject.SetActive(true);
            teamSelectorPanel.CreateTeamSelectorPanel(TeamSelectorPanel.Mode.Build, builder,
                LoadManager.Instance.allBuildingData[builder.Type].upgradePoint[builder.Level], callback, false);
            teamSelectorPanel.gameObject.GetComponent<ClosePanelHelper>().SetOnExitCallback(() =>
            {
                buildPermission = false;
                CancleShowAvailableTiles();
            });
        }
        else
        {
            Debug.Log($"No LaborCenter found. Using Default production point(5).");
            callback(0);
        }
    }
    void TeamSelectorCallback(Building.BuildingType type, int teamNumber, Vector2 position, int removingTileIndex)
    {
        if (BuildManager.Instance.CreateNewBuilding(type, teamNumber, position))
        {
            constructableGrid.RemoveAt(removingTileIndex);
        }

        buildPermission = false;
        CancleShowAvailableTiles();

        return;
    }
    public void ReclaimConstructableGrid(Builder builder)
    {

        Vector3Int[] allPosition = new Vector3Int[1];
        Vector3Int reclaimCellPosition = gridLayout.WorldToCell(builder.Position);
        allPosition[0] = new Vector3Int(reclaimCellPosition.x - BUILDING_POSITION_OFFSET, reclaimCellPosition.y - BUILDING_POSITION_OFFSET, 0);

        MapManager.Instance.constructableGrid.Add(allPosition);
    }
    public void LoadBuildingToScene()
    {
        PlayerData playerData = LoadManager.Instance.playerData;
        if (playerData == null)
        {
            return;
        }

        Builder[] buildingInScene = playerData.buildingInPossession.ToArray();
        if (buildingInScene == null)
        {
            return;
        }

        for (int i = 0; i < buildingInScene.Length; i++)
        {
            SelectedBuildingName = ((int)buildingInScene[i].Type).ToString();
            LoadTile(buildingInScene[i]);

        }
    }
    IEnumerator DelaySetCanvasActive(bool active)
    {
        yield return new WaitForSeconds(1f);
        MainCanvas.canvasActive = active;
    }
}
