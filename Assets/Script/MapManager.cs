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
    protected override void OnInitialize()
    {
        
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        EventManager.Instance.OnPlayerLevelUp += OnPlayerLevelUp;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        if(EventManager.Instance)
        {
            EventManager.Instance.OnPlayerLevelUp -= OnPlayerLevelUp;
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene" && secondCalled)
        {
            Awake();
            Start();
        }
        secondCalled = true;
    }
     void Start()    
    {
       tilemap.CompressBounds();
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
    GridLayout gridLayout;

    public Tile selectedTile;
    public Tile defaultTileMap;
    Dictionary<int, List<Vector3Int[]>> constructableGridDictionary;
    List<Vector3Int[]> constructableGrid;

    bool buildPermission;

    public void SetExpandedArea()
    {
        foreach(int treeID in LoadManager.Instance.playerData.expandedArea)
        {
            Debug.Log(treeID);
            GameObject treeGO = GameManager.FindDeepChild(GameObject.Find("Map/Trees").transform, treeID.ToString()).gameObject;
            Debug.Log(treeGO.name);
            if (treeGO)
            {
                Destroy(treeGO);
            }
            
        }

        for(int i = 1; i <= LoadManager.Instance.playerData.level/5; i++)
        {
            GameObject fogGO = GameObject.Find("Map/FogArea/" + i);
            if(fogGO)
            {
                Destroy(fogGO);
            }
        }

        return;
    }
    void OnPlayerLevelUp(int level)
    {
        if(level % 5 == 0)
        {
            GameObject fogGO = GameObject.Find("Map/FogArea/" + level/5);
            if (fogGO)
            {
                Destroy(fogGO);
            }
        }
    }
    public string SelectedBuildingName { get; set; }

    public void ClickTile(Vector3Int position)
    {
      //  Debug.Log(position.ToString());
        for (int i = 0; i < constructableGrid.Count; i++)
        {
            if (constructableGrid[i].Contains(position))
            {

               

                    /// Prevent from immediately showing Building Information Panel after build. 
                    MainCanvas.canvasActive = true;
                    Resources.FindObjectsOfTypeAll<BuildingShopPanel>()[0].gameObject.SetActive(false);
                    StartCoroutine(DelaySetCanvasActive(false));
                /// -------------------------------------------------------------------------------
               
                Action<int> callback = (teamNumber) => {
                    TeamSelectorCallback((Building.BuildingType)int.Parse(SelectedBuildingName),
                    teamNumber, position, i); };
                   
                Builder builder = new Builder((Building.BuildingType)int.Parse(SelectedBuildingName));
                ShowTeamSelectorPanel(builder, callback);

                return;
            }
        }      
    }


    public void LoadTile(Builder builder)
    {
        for (int i = 0; i < constructableGrid.Count; i++)
        {
            if (constructableGrid[i].Contains(gridLayout.WorldToCell(builder.Position)))
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

        Tile[] t = new Tile[4];
        for(int i = 0; i < t.Length; i ++)
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
        Tile[] t = new Tile[4];
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
      //  constructableGridDictionary.Add();

        constructableGrid = new List<Vector3Int[]>();
        for (int i = 0; i < 60; i++)
        {
            constructableGrid.Add(new Vector3Int[4]);

        }

        //แถบซ้ายบน
        constructableGrid[0][0] = new Vector3Int(18, 124, 0);
        constructableGrid[0][1] = new Vector3Int(18, 123, 0);
        constructableGrid[0][2] = new Vector3Int(17, 124, 0);
        constructableGrid[0][3] = new Vector3Int(17, 123, 0);

        constructableGrid[1][0] = new Vector3Int(34, 110, 0);
        constructableGrid[1][1] = new Vector3Int(34, 109, 0);
        constructableGrid[1][2] = new Vector3Int(33, 110, 0);
        constructableGrid[1][3] = new Vector3Int(33, 109, 0);

        constructableGrid[2][0] = new Vector3Int(18, 110, 0);
        constructableGrid[2][1] = new Vector3Int(18, 109, 0);
        constructableGrid[2][2] = new Vector3Int(17, 110, 0);
        constructableGrid[2][3] = new Vector3Int(17, 109, 0);

        constructableGrid[3][0] = new Vector3Int(3, 111, 0);
        constructableGrid[3][1] = new Vector3Int(3, 110, 0);
        constructableGrid[3][2] = new Vector3Int(2, 111, 0);
        constructableGrid[3][3] = new Vector3Int(2, 110, 0);

        constructableGrid[4][0] = new Vector3Int(-23, 85, 0);
        constructableGrid[4][1] = new Vector3Int(-23, 84, 0);
        constructableGrid[4][2] = new Vector3Int(-24, 85, 0);
        constructableGrid[4][3] = new Vector3Int(-24, 84, 0);

        constructableGrid[5][0] = new Vector3Int(6, 83, 0);
        constructableGrid[5][1] = new Vector3Int(6, 82, 0);
        constructableGrid[5][2] = new Vector3Int(5, 83, 0);
        constructableGrid[5][3] = new Vector3Int(5, 82, 0);

        constructableGrid[6][0] = new Vector3Int(22, 83, 0);
        constructableGrid[6][1] = new Vector3Int(22, 82, 0);
        constructableGrid[6][2] = new Vector3Int(21, 83, 0);
        constructableGrid[6][3] = new Vector3Int(21, 82, 0);

        constructableGrid[7][0] = new Vector3Int(36, 84, 0);
        constructableGrid[7][1] = new Vector3Int(36, 83, 0);
        constructableGrid[7][2] = new Vector3Int(35, 84, 0);
        constructableGrid[7][3] = new Vector3Int(35, 83, 0);

        constructableGrid[8][0] = new Vector3Int(63, 82, 0);
        constructableGrid[8][1] = new Vector3Int(63, 81, 0);
        constructableGrid[8][2] = new Vector3Int(62, 82, 0);
        constructableGrid[8][3] = new Vector3Int(62, 81, 0);

        constructableGrid[9][0] = new Vector3Int(-46, 62, 0);
        constructableGrid[9][1] = new Vector3Int(-46, 61, 0);
        constructableGrid[9][2] = new Vector3Int(-47, 62, 0);
        constructableGrid[9][3] = new Vector3Int(-47, 61, 0);

        constructableGrid[10][0] = new Vector3Int(-62, 44, 0);
        constructableGrid[10][1] = new Vector3Int(-62, 43, 0);
        constructableGrid[10][2] = new Vector3Int(-63, 44, 0);
        constructableGrid[10][3] = new Vector3Int(-63, 43, 0);

        constructableGrid[11][0] = new Vector3Int(-46, 43, 0);
        constructableGrid[11][1] = new Vector3Int(-46, 42, 0);
        constructableGrid[11][2] = new Vector3Int(-47, 43, 0);
        constructableGrid[11][3] = new Vector3Int(-47, 42, 0);

        constructableGrid[12][0] = new Vector3Int(-21, 57, 0);
        constructableGrid[12][1] = new Vector3Int(-21, 56, 0);
        constructableGrid[12][2] = new Vector3Int(-22, 57, 0);
        constructableGrid[12][3] = new Vector3Int(-22, 56, 0);

        constructableGrid[13][0] = new Vector3Int(-21, 43, 0);
        constructableGrid[13][1] = new Vector3Int(-21, 42, 0);
        constructableGrid[13][2] = new Vector3Int(-22, 43, 0);
        constructableGrid[13][3] = new Vector3Int(-22, 42, 0);

        constructableGrid[14][0] = new Vector3Int(7, 53, 0);
        constructableGrid[14][1] = new Vector3Int(7, 52, 0);
        constructableGrid[14][2] = new Vector3Int(6, 53, 0);
        constructableGrid[14][3] = new Vector3Int(6, 52, 0);

        constructableGrid[15][0] = new Vector3Int(19, 42, 0);
        constructableGrid[15][1] = new Vector3Int(19, 41, 0);
        constructableGrid[15][2] = new Vector3Int(18, 42, 0);
        constructableGrid[15][3] = new Vector3Int(18, 41, 0);

        constructableGrid[16][0] = new Vector3Int(35, 55, 0);
        constructableGrid[16][1] = new Vector3Int(35, 54, 0);
        constructableGrid[16][2] = new Vector3Int(34, 55, 0);
        constructableGrid[16][3] = new Vector3Int(34, 54, 0);

        constructableGrid[17][0] = new Vector3Int(66, 53, 0);
        constructableGrid[17][1] = new Vector3Int(66, 52, 0);
        constructableGrid[17][2] = new Vector3Int(65, 53, 0);
        constructableGrid[17][3] = new Vector3Int(65, 52, 0);

        constructableGrid[18][0] = new Vector3Int(19, 14, 0);
        constructableGrid[18][1] = new Vector3Int(19, 13, 0);
        constructableGrid[18][2] = new Vector3Int(18, 14, 0);
        constructableGrid[18][3] = new Vector3Int(18, 13, 0);

        constructableGrid[19][0] = new Vector3Int(-4, 5, 0);
        constructableGrid[19][1] = new Vector3Int(-4, 4, 0);
        constructableGrid[19][2] = new Vector3Int(-5, 5, 0);
        constructableGrid[19][3] = new Vector3Int(-5, 4, 0);

        constructableGrid[20][0] = new Vector3Int(-21, 12, 0);
        constructableGrid[20][1] = new Vector3Int(-21, 11, 0);
        constructableGrid[20][2] = new Vector3Int(-22, 12, 0);
        constructableGrid[20][3] = new Vector3Int(-22, 11, 0);



        //แถวซ้ายล่าง
        constructableGrid[21][0] = new Vector3Int(-77, 22, 0);
        constructableGrid[21][1] = new Vector3Int(-77, 21, 0);
        constructableGrid[21][2] = new Vector3Int(-78, 22, 0);
        constructableGrid[21][3] = new Vector3Int(-78, 21, 0);

        constructableGrid[22][0] = new Vector3Int(-97, 8, 0);
        constructableGrid[22][1] = new Vector3Int(-97, 7, 0);
        constructableGrid[22][2] = new Vector3Int(-98, 8, 0);
        constructableGrid[22][3] = new Vector3Int(-98, 7, 0);

        constructableGrid[23][0] = new Vector3Int(-79, 3, 0);
        constructableGrid[23][1] = new Vector3Int(-79, 2, 0);
        constructableGrid[23][2] = new Vector3Int(-80, 3, 0);
        constructableGrid[23][3] = new Vector3Int(-80, 2, 0);

        constructableGrid[24][0] = new Vector3Int(-46, 16, 0);
        constructableGrid[24][1] = new Vector3Int(-46, 15, 0);
        constructableGrid[24][2] = new Vector3Int(-47, 16, 0);
        constructableGrid[24][3] = new Vector3Int(-47, 15, 0);

        constructableGrid[25][0] = new Vector3Int(-47, 2, 0);
        constructableGrid[25][1] = new Vector3Int(-47, 1, 0);
        constructableGrid[25][2] = new Vector3Int(-48, 2, 0);
        constructableGrid[25][3] = new Vector3Int(-48, 1, 0);

        constructableGrid[26][0] = new Vector3Int(-116, -26, 0);
        constructableGrid[26][1] = new Vector3Int(-116, -27, 0);
        constructableGrid[26][2] = new Vector3Int(-117, -26, 0);
        constructableGrid[26][3] = new Vector3Int(-117, -27, 0);

        constructableGrid[27][0] = new Vector3Int(-98, -26, 0);
        constructableGrid[27][1] = new Vector3Int(-98, -27, 0);
        constructableGrid[27][2] = new Vector3Int(-99, -26, 0);
        constructableGrid[27][3] = new Vector3Int(-99, -27, 0);

        constructableGrid[28][0] = new Vector3Int(-80, -28, 0);
        constructableGrid[28][1] = new Vector3Int(-80, -29, 0);
        constructableGrid[28][2] = new Vector3Int(-81, -28, 0);
        constructableGrid[28][3] = new Vector3Int(-81, -29, 0);

        constructableGrid[29][0] = new Vector3Int(-99, -45, 0);
        constructableGrid[29][1] = new Vector3Int(-99, -46, 0);
        constructableGrid[29][2] = new Vector3Int(-100, -45, 0);
        constructableGrid[29][3] = new Vector3Int(-100, -46, 0);

        constructableGrid[30][0] = new Vector3Int(-78, -50, 0);
        constructableGrid[30][1] = new Vector3Int(-78, -51, 0);
        constructableGrid[30][2] = new Vector3Int(-79, -50, 0);
        constructableGrid[30][3] = new Vector3Int(-79, -51, 0);

        constructableGrid[31][0] = new Vector3Int(-44, -45, 0);
        constructableGrid[31][1] = new Vector3Int(-44, -46, 0);
        constructableGrid[31][2] = new Vector3Int(-45, -45, 0);
        constructableGrid[31][3] = new Vector3Int(-45, -46, 0);

        constructableGrid[32][0] = new Vector3Int(-68, -74, 0);
        constructableGrid[32][1] = new Vector3Int(-68, -75, 0);
        constructableGrid[32][2] = new Vector3Int(-69, -74, 0);
        constructableGrid[32][3] = new Vector3Int(-69, -75, 0);


        //แถบขวาบน
        constructableGrid[33][0] = new Vector3Int(88, 50, 0);
        constructableGrid[33][1] = new Vector3Int(88, 49, 0);
        constructableGrid[33][2] = new Vector3Int(87, 50, 0);
        constructableGrid[33][3] = new Vector3Int(87, 49, 0);

        constructableGrid[34][0] = new Vector3Int(71, 33, 0);
        constructableGrid[34][1] = new Vector3Int(71, 32, 0);
        constructableGrid[34][2] = new Vector3Int(70, 33, 0);
        constructableGrid[34][3] = new Vector3Int(70, 32, 0);

        constructableGrid[35][0] = new Vector3Int(92, 33, 0);
        constructableGrid[35][1] = new Vector3Int(92, 32, 0);
        constructableGrid[35][2] = new Vector3Int(91, 33, 0);
        constructableGrid[35][3] = new Vector3Int(91, 32, 0);

        constructableGrid[36][0] = new Vector3Int(66, 13, 0);
        constructableGrid[36][1] = new Vector3Int(66, 12, 0);
        constructableGrid[36][2] = new Vector3Int(65, 13, 0);
        constructableGrid[36][3] = new Vector3Int(65, 12, 0);

        constructableGrid[37][0] = new Vector3Int(112, 30, 0);
        constructableGrid[37][1] = new Vector3Int(112, 29, 0);
        constructableGrid[37][2] = new Vector3Int(111, 30, 0);
        constructableGrid[37][3] = new Vector3Int(111, 29, 0);

        constructableGrid[38][0] = new Vector3Int(113, 9, 0);
        constructableGrid[38][1] = new Vector3Int(113, 8, 0);
        constructableGrid[38][2] = new Vector3Int(112, 9, 0);
        constructableGrid[38][3] = new Vector3Int(112, 8, 0);

        constructableGrid[39][0] = new Vector3Int(87, 8, 0);
        constructableGrid[39][1] = new Vector3Int(87, 7, 0);
        constructableGrid[39][2] = new Vector3Int(86, 8, 0);
        constructableGrid[39][3] = new Vector3Int(86, 7, 0);

        constructableGrid[40][0] = new Vector3Int(36, 4, 0);
        constructableGrid[40][1] = new Vector3Int(36, 3, 0);
        constructableGrid[40][2] = new Vector3Int(35, 4, 0);
        constructableGrid[40][3] = new Vector3Int(35, 3, 0);

        constructableGrid[41][0] = new Vector3Int(65, -27, 0);
        constructableGrid[41][1] = new Vector3Int(65, -28, 0);
        constructableGrid[41][2] = new Vector3Int(64, -27, 0);
        constructableGrid[41][3] = new Vector3Int(64, -28, 0);

        constructableGrid[42][0] = new Vector3Int(83, -26, 0);
        constructableGrid[42][1] = new Vector3Int(83, -27, 0);
        constructableGrid[42][2] = new Vector3Int(82, -26, 0);
        constructableGrid[42][3] = new Vector3Int(82, -27, 0);

        constructableGrid[43][0] = new Vector3Int(65, -42, 0);
        constructableGrid[43][1] = new Vector3Int(65, -43, 0);
        constructableGrid[43][2] = new Vector3Int(64, -42, 0);
        constructableGrid[43][3] = new Vector3Int(64, -43, 0);

        constructableGrid[44][0] = new Vector3Int(11, -26, 0);
        constructableGrid[44][1] = new Vector3Int(11, -27, 0);
        constructableGrid[44][2] = new Vector3Int(10, -26, 0);
        constructableGrid[44][3] = new Vector3Int(10, -27, 0);

        constructableGrid[45][0] = new Vector3Int(33, -29, 0);
        constructableGrid[45][1] = new Vector3Int(33, -30, 0);
        constructableGrid[45][2] = new Vector3Int(32, -29, 0);
        constructableGrid[45][3] = new Vector3Int(32, -30, 0);

        constructableGrid[46][0] = new Vector3Int(16, -44, 0);
        constructableGrid[46][1] = new Vector3Int(16, -45, 0);
        constructableGrid[46][2] = new Vector3Int(15, -44, 0);
        constructableGrid[46][3] = new Vector3Int(15, -45, 0);

        constructableGrid[47][0] = new Vector3Int(38, -46, 0);
        constructableGrid[47][1] = new Vector3Int(38, -47, 0);
        constructableGrid[47][2] = new Vector3Int(37, -46, 0);
        constructableGrid[47][3] = new Vector3Int(37, -47, 0);


        //แถวขวาล่าง
        constructableGrid[48][0] = new Vector3Int(-30, -28, 0);
        constructableGrid[48][1] = new Vector3Int(-30, -29, 0);
        constructableGrid[48][2] = new Vector3Int(-31, -28, 0);
        constructableGrid[48][3] = new Vector3Int(-31, -29, 0);

        constructableGrid[49][0] = new Vector3Int(-18, -42, 0);
        constructableGrid[49][1] = new Vector3Int(-18, -43, 0);
        constructableGrid[49][2] = new Vector3Int(-19, -42, 0);
        constructableGrid[49][3] = new Vector3Int(-19, -43, 0);


        constructableGrid[50][0] = new Vector3Int(-55, -84, 0);
        constructableGrid[50][1] = new Vector3Int(-55, -85, 0);
        constructableGrid[50][2] = new Vector3Int(-56, -84, 0);
        constructableGrid[50][3] = new Vector3Int(-56, -85, 0);

        constructableGrid[51][0] = new Vector3Int(-41, -73, 0);
        constructableGrid[51][1] = new Vector3Int(-41, -74, 0);
        constructableGrid[51][2] = new Vector3Int(-42, -73, 0);
        constructableGrid[51][3] = new Vector3Int(-42, -74, 0);

        constructableGrid[52][0] = new Vector3Int(-30, -84, 0);
        constructableGrid[52][1] = new Vector3Int(-30, -85, 0);
        constructableGrid[52][2] = new Vector3Int(-31, -84, 0);
        constructableGrid[52][3] = new Vector3Int(-31, -85, 0);

        constructableGrid[53][0] = new Vector3Int(-14, -72, 0);
        constructableGrid[53][1] = new Vector3Int(-14, -73, 0);
        constructableGrid[53][2] = new Vector3Int(-15, -72, 0);
        constructableGrid[53][3] = new Vector3Int(-15, -73, 0);

        constructableGrid[54][0] = new Vector3Int(34, -74, 0);
        constructableGrid[54][1] = new Vector3Int(34, -75, 0);
        constructableGrid[54][2] = new Vector3Int(33, -74, 0);
        constructableGrid[54][3] = new Vector3Int(33, -75, 0);

        constructableGrid[55][0] = new Vector3Int(16, -75, 0);
        constructableGrid[55][1] = new Vector3Int(16, -76, 0);
        constructableGrid[55][2] = new Vector3Int(15, -75, 0);
        constructableGrid[55][3] = new Vector3Int(15, -76, 0);

        constructableGrid[56][0] = new Vector3Int(13, -93, 0);
        constructableGrid[56][1] = new Vector3Int(13, -94, 0);
        constructableGrid[56][2] = new Vector3Int(12, -93, 0);
        constructableGrid[56][3] = new Vector3Int(12, -94, 0);

        constructableGrid[57][0] = new Vector3Int(-6, -113, 0);
        constructableGrid[57][1] = new Vector3Int(-6, -114, 0);
        constructableGrid[57][2] = new Vector3Int(-7, -113, 0);
        constructableGrid[57][3] = new Vector3Int(-7, -114, 0);

        constructableGrid[58][0] = new Vector3Int(-28, -114, 0);
        constructableGrid[58][1] = new Vector3Int(-28, -115, 0);
        constructableGrid[58][2] = new Vector3Int(-29, -114, 0);
        constructableGrid[58][3] = new Vector3Int(-29, -115, 0);

        constructableGrid[59][0] = new Vector3Int(-18, -126, 0);
        constructableGrid[59][1] = new Vector3Int(-18, -127, 0);
        constructableGrid[59][2] = new Vector3Int(-19, -126, 0);
        constructableGrid[59][3] = new Vector3Int(-19, -127, 0);

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
            teamSelectorPanel.gameObject.GetComponent<ClosePanelHelper>().SetOnExitCallback(()=> {
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
    void TeamSelectorCallback(Building.BuildingType type, int teamNumber, Vector3Int position, int removingTileIndex)
    {
        if(BuildManager.Instance.CreateNewBuilding(type, teamNumber, gridLayout.CellToWorld(position)))
        {
            constructableGrid.RemoveAt(removingTileIndex);
        }

        buildPermission = false;
        CancleShowAvailableTiles();

        return;
    }
    public void ReclaimConstructableGrid(Builder builder)
    {

        Vector3Int[] position = new Vector3Int[1];
        position[0] = gridLayout.WorldToCell(builder.Position);
        MapManager.Instance.constructableGrid.Add(position);
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
        yield return new WaitForEndOfFrame();
        MainCanvas.canvasActive = active;
    }
}
