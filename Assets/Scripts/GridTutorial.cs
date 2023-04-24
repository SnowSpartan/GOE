using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class GridTutorial : MonoBehaviour
{
    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();
    [SerializeField] private GridHeatMap gridHeatMap;
    //[SerializeField] private List<Character> charactersList;
    //[SerializeField] private GameObject mapPlayer;
    [SerializeField] private GameObject battlePlayer;
    [SerializeField] private GameObject GameOver;
    [SerializeField] private AudioClip encounter;
    [SerializeField] private RingMenuMB characterMenuMB;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private PlayerInput playerInput;
    //[SerializeField] private RingMenu characterMenuPrefab;
    protected RingMenu characterMenuInterface;

    //private ActionBasedController controller;
    //private GameObject gameManager;
    //private GameObject mapPlayer;
    private Grid<HeatMapGridObject> heatGrid;
    private Grid<GridObject> grid;
    private Character character;
    private Character.Dir dir = Character.Dir.Down;
    private PlacedCharacter playerCharacter;
    [SerializeField] private List<Character> characterList;
    private List<PlacedCharacter> allyList;
    private List<Vector2Int> allyPositionsList;
    private List<PlacedCharacter> enemyList;
    private List<Vector2Int> enemyPositionsList;
    private Pathfinding pathfinding;
    //private enum gameState;
    private float characterWidth;
    private float characterHeight;
    private int width;
    private int height;
    private int range;
    private int turnCount;
    private bool flag1;

    /*private enum GameState
    {
        //Battle: true,
        //Move: false,
        //Menu: false
        Battle,
        Move,
        Menu
    }*/

    private void Start()
    {
        width = 10;
        height = 10;
        range = 3;
        turnCount = 1;
        flag1 = true;

        //controller = GetComponent<ActionBasedController>();
        //gameManager = GameObject.Find("GameManager");
        characterMenuMB.Despawn();

        pathfinding = new Pathfinding(10, 10);
        heatGrid = new Grid<HeatMapGridObject>(width, height, 5f, new Vector3(-75, 0, -75), (Grid<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));//<int>(10, 10, 5f, new Vector3(-75, 0, -75));
        grid = new Grid<GridObject>(width, height, 5f, new Vector3(-75, 0, -75), (Grid<GridObject> g, int x, int y) => new GridObject(g, x, y));
        gridHeatMap.SetGrid(heatGrid);

        battlePlayer.transform.position = new Vector3(-55, 4, -35);
        battlePlayer.GetComponent<AudioSource>().Play();

        //characterList = gameManager.GetComponent<GameManager>().GetCurrentEncounter();
        enemyList = new List<PlacedCharacter>();
        enemyPositionsList = new List<Vector2Int>();
        allyList = new List<PlacedCharacter>();
        allyPositionsList = new List<Vector2Int>();

        character = characterList[0];
        characterWidth = 0.5f;
        characterHeight = 0.5f;
        dir = Character.GetNextDir(dir);
        dir = Character.GetNextDir(dir);

        for (int i = 0; i < characterList.Count; i++)
        {
            character = characterList[i];
            SpawnCharacter(character, dir);

        }
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int y;
        private PlacedCharacter placedCharacter;

        public GridObject(Grid<GridObject> grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetPlacedCharacter(PlacedCharacter placedCharacter)
        {
            this.placedCharacter = placedCharacter;
            grid.TriggerGridObjectChanged(x, y);
        }

        public void ClearPlacedCharacter()
        {
            placedCharacter = null;
            grid.TriggerGridObjectChanged(x, y);
        }

        public PlacedCharacter GetPlacedCharacter()
        {
            return placedCharacter;
        }

        public bool CanMove()
        {
            return placedCharacter == null;
        }
    }

    private void OnFire()
    {
        //if (Input.GetMouseButtonDown(0))// || controller.selectAction.action.ReadValue<bool>())
        //{
        heatGrid.GetXY(GetPointerWorldPosition(), out int originX, out int originY);

        GridObject gridObject = grid.GetValue(originX, originY);
        PlacedCharacter unknownCharacter = gridObject.GetPlacedCharacter();
        List<Vector2Int> gridPositionList = character.GetGridPositionList(new Vector2Int(originX, originY), dir);//Might not work as intended
        bool canMove = true;
        var menuName = characterMenuMB.GetName();

        //pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int endX, out int endY);
        HeatMapGridObject heatMapGridObject = heatGrid.GetValue(originX, originY);

        if (characterMenuMB.activeSelf())
        {
            if (menuName == "Move")
            {
                originX = playerCharacter.GetOrigin().x;
                originY = playerCharacter.GetOrigin().y;

                for (int i = 0; i < range; i++)
                {
                    //pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);

                    for (int j = 0; j < range - i; j++)
                    {
                        //if ((i - 1) < 0 || (j - 1) < 0 || (j + 1) > range || (i + 1) > range)
                        //{
                        if (heatGrid.GetValue(originX + i, originY + j) != null)
                        {
                            //pathfinding = pathfinding.GetGrid().GetValue(originX + i, originY + j);
                            if (pathfinding.GetNode(originX + i, originY + j).isWalkable)// && heatGrid.GetValue(originX + i, originY + j) != null)
                            {
                                heatMapGridObject = heatGrid.GetValue(originX + i, originY + j);
                                heatMapGridObject.AddValue(40, width, height);
                            }
                            else if (grid.GetValue(originX + i, originY + j).GetPlacedCharacter() != null)
                            {
                                if (grid.GetValue(originX + i, originY + j).GetPlacedCharacter().GetAlliegence())
                                {
                                    heatMapGridObject = heatGrid.GetValue(originX + i, originY + j);
                                    heatMapGridObject.AddValue(0, width, height);
                                }
                            }
                            if (i != 0)
                            {
                                if (heatGrid.GetValue(originX - i, originY + j) != null)
                                {
                                    if (pathfinding.GetNode(originX - i, originY + j).isWalkable)// && heatGrid.GetValue(originX - i, originY + j) != null)
                                    {
                                        heatMapGridObject = heatGrid.GetValue(originX - i, originY + j);
                                        heatMapGridObject.AddValue(40, width, height);
                                    }
                                    else if (grid.GetValue(originX - i, originY + j).GetPlacedCharacter() != null)
                                    {
                                        if (grid.GetValue(originX - i, originY + j).GetPlacedCharacter().GetAlliegence())
                                        {
                                            heatMapGridObject = heatGrid.GetValue(originX - i, originY + j);
                                            heatMapGridObject.AddValue(0, width, height);
                                        }
                                    }
                                }
                            }
                            if (j != 0)
                            {
                                if (heatGrid.GetValue(originX + i, originY - j) != null)
                                {
                                    if (pathfinding.GetNode(originX + i, originY - j).isWalkable)// && heatGrid.GetValue(originX + i, originY - j) != null)
                                    {
                                        heatMapGridObject = heatGrid.GetValue(originX + i, originY - j);
                                        heatMapGridObject.AddValue(40, width, height);
                                    }
                                    else if (grid.GetValue(originX + i, originY - j).GetPlacedCharacter() != null)
                                    {
                                        if (grid.GetValue(originX + i, originY - j).GetPlacedCharacter().GetAlliegence())
                                        {
                                            heatMapGridObject = heatGrid.GetValue(originX + i, originY - j);
                                            heatMapGridObject.AddValue(0, width, height);
                                        }
                                    }
                                    if (i != 0)
                                    {
                                        if (heatGrid.GetValue(originX - i, originY - j) != null)
                                        {
                                            if (pathfinding.GetNode(originX - i, originY - i).isWalkable)
                                            {
                                                heatMapGridObject = heatGrid.GetValue(originX - i, originY - i);
                                                heatMapGridObject.AddValue(40, width, height);
                                            }
                                            else if (grid.GetValue(originX - i, originY - j).GetPlacedCharacter() != null)
                                            {
                                                if (grid.GetValue(originX - i, originY - j).GetPlacedCharacter().GetAlliegence())
                                                {
                                                    heatMapGridObject = heatGrid.GetValue(originX - i, originY - j);
                                                    heatMapGridObject.AddValue(0, width, height);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                }
                gridHeatMap.UpdateHeatMap();

                //playerCharacter = gridObject.GetPlacedCharacter();
                flag1 = false;
                characterMenuMB.Despawn();
            }
            else if (menuName == "Attack")
            {
                originX = playerCharacter.GetOrigin().x;
                originY = playerCharacter.GetOrigin().y;

                for (int i = 0; i < range; i++)
                {
                    //pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);

                    for (int j = 0; j < range - i; j++)
                    {
                        //if ((i - 1) < 0 || (j - 1) < 0 || (j + 1) > range || (i + 1) > range)
                        //{
                        if (heatGrid.GetValue(originX + i, originY + j) != null)
                        {
                            //pathfinding = pathfinding.GetGrid().GetValue(originX + i, originY + j);
                            if (pathfinding.GetNode(originX + i, originY + j).isWalkable)// && heatGrid.GetValue(originX + i, originY + j) != null)
                            {
                                heatMapGridObject = heatGrid.GetValue(originX + i, originY + j);
                                heatMapGridObject.AddValue(20, width, height);
                            }
                            else if (grid.GetValue(originX + i, originY + j).GetPlacedCharacter() != null)
                            {
                                if (grid.GetValue(originX + i, originY + j).GetPlacedCharacter().GetAlliegence())
                                {
                                    heatMapGridObject = heatGrid.GetValue(originX + i, originY + j);
                                    heatMapGridObject.AddValue(5, width, height);
                                }
                            }
                            if (i != 0)
                            {
                                if (heatGrid.GetValue(originX - i, originY + j) != null)
                                {
                                    if (pathfinding.GetNode(originX - i, originY + j).isWalkable)// && heatGrid.GetValue(originX - i, originY + j) != null)
                                    {
                                        heatMapGridObject = heatGrid.GetValue(originX - i, originY + j);
                                        heatMapGridObject.AddValue(20, width, height);
                                    }
                                    else if (grid.GetValue(originX - i, originY + j).GetPlacedCharacter() != null)
                                    {
                                        if (grid.GetValue(originX - i, originY + j).GetPlacedCharacter().GetAlliegence())
                                        {
                                            heatMapGridObject = heatGrid.GetValue(originX - i, originY + j);
                                            heatMapGridObject.AddValue(5, width, height);
                                        }
                                    }
                                }
                            }
                            if (j != 0)
                            {
                                if (heatGrid.GetValue(originX + i, originY - j) != null)
                                {
                                    if (pathfinding.GetNode(originX + i, originY - j).isWalkable)// && heatGrid.GetValue(originX + i, originY - j) != null)
                                    {
                                        heatMapGridObject = heatGrid.GetValue(originX + i, originY - j);
                                        heatMapGridObject.AddValue(20, width, height);
                                    }
                                    else if (grid.GetValue(originX + i, originY - j).GetPlacedCharacter() != null)
                                    {
                                        if (grid.GetValue(originX + i, originY - j).GetPlacedCharacter().GetAlliegence())
                                        {
                                            heatMapGridObject = heatGrid.GetValue(originX + i, originY - j);
                                            heatMapGridObject.AddValue(5, width, height);
                                        }
                                    }
                                    if (i != 0)
                                    {
                                        if (heatGrid.GetValue(originX - i, originY - j) != null)
                                        {
                                            if (pathfinding.GetNode(originX - i, originY - i).isWalkable)
                                            {
                                                heatMapGridObject = heatGrid.GetValue(originX - i, originY - i);
                                                heatMapGridObject.AddValue(20, width, height);
                                            }
                                            else if (grid.GetValue(originX - i, originY - j).GetPlacedCharacter() != null)
                                            {
                                                if (grid.GetValue(originX - i, originY - j).GetPlacedCharacter().GetAlliegence())
                                                {
                                                    heatMapGridObject = heatGrid.GetValue(originX - i, originY - j);
                                                    heatMapGridObject.AddValue(5, width, height);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //}
                    }
                }
                gridHeatMap.UpdateHeatMap();

                //playerCharacter = gridObject.GetPlacedCharacter();
                flag1 = false;
                characterMenuMB.Despawn();
            }
        }
        else
        {
            if (heatMapGridObject.GetValue() == 0 && flag1 && unknownCharacter != null)
            {
                if (!unknownCharacter.GetAlliegence() && unknownCharacter.GetCanMove())
                {
                    characterMenuMB.Spawn();
                    //gridObject.GetPlacedCharacter().SetCanMove(false);
                    playerCharacter = gridObject.GetPlacedCharacter();
                }
            }
            else if (heatMapGridObject.GetValue() == 0 && !flag1)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        heatMapGridObject = heatGrid.GetValue(i, j);
                        heatMapGridObject.AddValue(0, width, height);
                    }
                }

                gridHeatMap.UpdateHeatMap();
                flag1 = true;
            }
            else if (heatMapGridObject.GetValue() == 5 && unknownCharacter != null && menuName == "Attack")//Attack if enemy is selected, do nothing if ally is selected
            {
                if (unknownCharacter.GetAlliegence() && unknownCharacter.GetCanMove())
                {
                    playerCharacter.GetGameObject().GetComponent<VisualEffect>().Stop();
                    playerCharacter.GetGameObject().GetComponent<AudioSource>().Play();
                    //Debug.Log(playerCharacter.GetName());

                    float dmg = playerCharacter.GetDamage();
                    playerCharacter.SetCanMove(false);
                    unknownCharacter.ChangeHealth(dmg);

                    if (unknownCharacter.GetHealth() <= 0)
                    {
                        Debug.Log("Dead");
                        //unknownCharacter.RestoreHealth();
                        unknownCharacter.DestroySelf();

                        gridPositionList = unknownCharacter.GetGridPositionList();

                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetValue(gridPosition.x, gridPosition.y).ClearPlacedCharacter();
                        }
                    }

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            heatMapGridObject = heatGrid.GetValue(i, j);
                            heatMapGridObject.AddValue(0, width, height);
                        }
                    }

                    gridHeatMap.UpdateHeatMap();

                    turnCount++;
                    flag1 = true;
                }
            }
            else if (heatMapGridObject.GetValue() == 40 && menuName == "Move")
            {

                gridPositionList = character.GetGridPositionList(new Vector2Int(originX, originY), dir);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    if (!grid.GetValue(gridPosition.x, gridPosition.y).CanMove())
                    {
                        canMove = false;
                        break;
                    }
                }

                if (canMove)
                {
                    playerCharacter.GetCharacter().GetPrefab().GetComponent<VisualEffect>().Stop();

                    character = playerCharacter.GetCharacter();
                    float instanceHealth = playerCharacter.GetHealth();
                    SetCharacterOffset(character);

                    PlacedCharacter newPlayerCharacter = PlacedCharacter.Create(
                        grid.GetWorldPosition(originX, originY) + new Vector3(grid.GetCellSize() * characterWidth, 1.6f, grid.GetCellSize() * characterHeight),
                        new Vector2Int(originX, originY),
                        dir,
                        character
                    );

                    newPlayerCharacter.SetHealth(instanceHealth);

                    foreach (Vector2Int gridPosition in gridPositionList)//for characters who are larger than 1x1, though no player controlled character can be
                    {
                        grid.GetValue(gridPosition.x, gridPosition.y).SetPlacedCharacter(newPlayerCharacter);
                        //pathfinding.GetNode(gridPosition.x, gridPosition.y).SetIsWalkable(false);
                    }
                    //Debug.Log(grid.GetValue(gridPositionList[0].x, gridPositionList[0].y).GetPlacedCharacter());

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            heatMapGridObject = heatGrid.GetValue(i, j);
                            heatMapGridObject.AddValue(0, width, height);
                        }
                    }

                    gridHeatMap.UpdateHeatMap();

                    for (int i = 0; i < allyList.Count; i++)
                    {
                        if (allyList[i] == playerCharacter)
                        {
                            allyList[i] = newPlayerCharacter;
                            allyPositionsList[i] = new Vector2Int(originX, originY);
                        }
                    }

                    PlacedCharacter deleteCharacter = playerCharacter;
                    playerCharacter = newPlayerCharacter;
                    playerCharacter.SetCanMove(false);
                    deleteCharacter.DestroySelf();
                    turnCount++;
                    flag1 = true;
                }
            }
        }

        if (turnCount > allyList.Count)
        {
            if (enemyList != null)
            {
                int target = 0;//Random.Range(0, allyList.Count);
                for (int n = 0; n < enemyList.Count; n++)
                {
                    target = Random.Range(0, allyList.Count);
                    for (int m = 0; m < allyList.Count; m++)
                    {
                        if (target == m)//makes all enemies target one ally
                        {
                            //for (int n = 0; n < enemyList.Count; n++)
                            //{
                            List<PathNode> enemyPath = pathfinding.FindPath(enemyPositionsList[n].x, enemyPositionsList[n].y, allyPositionsList[m].x, allyPositionsList[m].y);
                            PlacedCharacter tempEnemy = enemyList[n];
                            Character enemyCharacter = enemyList[n].GetCharacter();
                            SetCharacterOffset(enemyCharacter);
                            float instanceHealth = enemyList[n].GetHealth();
                            pathfinding.GetNode(enemyPositionsList[n].x, enemyPositionsList[n].y).SetIsWalkable(true);

                            if (grid.GetValue(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).GetPlacedCharacter() == null)
                            {
                                pathfinding.GetNode(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).SetIsWalkable(false);

                                PlacedCharacter newEnemyCharacter = PlacedCharacter.Create(
                                    grid.GetWorldPosition(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y) + new Vector3(grid.GetCellSize() * .5f, 1.6f, grid.GetCellSize() * .5f),
                                    new Vector2Int(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y),
                                    dir,
                                    enemyCharacter
                                );

                                newEnemyCharacter.SetHealth(instanceHealth);

                                grid.GetValue(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).SetPlacedCharacter(newEnemyCharacter);
                                enemyList[n] = newEnemyCharacter;
                                enemyPositionsList[n] = new Vector2Int(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y);

                                tempEnemy.DestroySelf();
                            }
                            else
                            {
                                float dmg = enemyList[n].GetDamage();

                                pathfinding.GetNode(enemyPositionsList[n].x, enemyPositionsList[n].y).SetIsWalkable(false);
                                allyList[m].ChangeHealth(dmg);
                                Debug.Log(dmg + " dmg, current health: " + allyList[m].GetHealth());

                                if (allyList[m].GetHealth() <= 0)
                                {
                                    Debug.Log("Dead");
                                    //unknownCharacter.RestoreHealth();

                                    grid.GetValue(allyPositionsList[m].x, allyPositionsList[m].y).ClearPlacedCharacter();

                                    allyList[m].DestroySelf();
                                    allyList.RemoveAt(m);
                                    allyPositionsList.RemoveAt(m);
                                    Debug.Log(allyList.Count);
                                }
                            }
                            /*PlacedCharacter newEnemyCharacter = PlacedCharacter.Create(
                                grid.GetWorldPosition(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y) + new Vector3(grid.GetCellSize() * .5f, 1.6f, grid.GetCellSize() * .5f),
                                new Vector2Int(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y),
                                dir,
                                enemyCharacter
                            );

                            newEnemyCharacter.SetHealth(instanceHealth);
                            Debug.Log(enemyPath.Count);
                            //Use for loop to make the enemy stop a space away from player, rather than stop entirely if they are in range of player. Should be able to replace 1 with i to make it stop 1 earlier in enemyPath list
                            //As a part of this, figure out how you made the enemy's movements 1.
                            if (grid.GetValue(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).GetPlacedCharacter() == null)
                            {
                                grid.GetValue(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).SetPlacedCharacter(newEnemyCharacter);
                                //pathfinding.GetNode(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y).SetIsWalkable(false);
                            }
                            enemyList[n] = newEnemyCharacter;
                            enemyPositionsList[n] = new Vector2Int(enemyPath[enemyPath.Count - 1].x, enemyPath[enemyPath.Count - 1].y);

                            tempEnemy.DestroySelf();*/
                            //Debug.Log(newEnemyCharacter.GetHealth());
                            //}
                        }
                    }
                }
            }
            //Debug.Log(enemyList[0].GetHealth());
            flag1 = false;
            turnCount = 1;
            for (int i = 0; i < allyList.Count; i++)
            {
                allyList[i].SetCanMove(true);
            }
        }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemyList[i].GetHealth() <= 0f)
            {
                //enemyList[i].RestoreHealth();
                pathfinding.GetNode(enemyPositionsList[i].x, enemyPositionsList[i].y).SetIsWalkable(true);
                enemyList[i].DestroySelf();
                enemyList.RemoveAt(i);
                enemyPositionsList.RemoveAt(i);
            }
        }
        if (enemyList.Count == 0)
        {
            //battlePlayer.SetActive(false);
            //mapPlayer.SetActive(true);
            //int currentEnemy = gameManager.GetComponent<GameManager>().GetCurrentEnemy();
            //gameManager.GetComponent<GameManager>().SetEnemies(currentEnemy);
            //gameManager.GetComponent<GameManager>().SetPartyState(true);
            SceneManager.LoadScene("GOE");
        }
        if (allyList.Count == 0)
        {
            //gameManager.GetComponent<GameManager>().SetPartyState(false);
            //GameOver.SetActive(true);
            //DontDestroyOnLoad(this.GameOver);
            SceneManager.LoadScene("Test");
        }
        //}

        /*if (Input.GetMouseButtonDown(1))
        {
            GridObject gridObject = grid.GetValue(GetMouseWorldPosition());
            PlacedCharacter placedCharacter = gridObject.GetPlacedCharacter();

            if (placedCharacter != null)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (placedCharacter == enemyList[i])
                    {

                        enemyList.RemoveAt(i);
                        enemyPositionsList.RemoveAt(i);
                    }
                    else if (placedCharacter == allyList[i])
                    {
                        allyList.RemoveAt(i);
                        allyPositionsList.RemoveAt(i);
                    }
                }
                placedCharacter.DestroySelf();

                List<Vector2Int> gridPositionList = placedCharacter.GetGridPositionList();//character.GetGridPositionList(new Vector2Int(originX, originY), dir);

                foreach (Vector2Int gridPosition in gridPositionList)
                {
                    grid.GetValue(gridPosition.x, gridPosition.y).ClearPlacedCharacter();
                }

            }


            pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);
            pathfinding.GetNode(x, y).SetIsWalkable(!pathfinding.GetNode(x, y).isWalkable);
        }*/


        /*if (Input.GetKeyDown(KeyCode.R))
        {
            dir = Character.GetNextDir(dir);
            Debug.Log(dir);
        }*/
    }
    /*
    private void PlayerTurnOrder(PlacedCharacter player)
    {
        player.SetCanMove(false);
        List<PlacedCharacter> tempAllyList = allyList;

        for (int i = 0; i < allyList.Count; i++)
        {
            if (allyList[i].GetCanMove())
            {
                for(int j = 0; j < i; j++)
                {
                    allyList[j].SetCanMove(false);
                }
                break;
            }

            allyList[i].SetCanMove(true);
        }
    }
    */
    private void SetCharacterOffset(Character character)
    {
        if (character.width > 1)
        {
            if (dir == Character.Dir.Left)// || dir == Character.Dir.Right)
            {
                characterWidth = (float)character.width / 2;
            }
            else if (dir == Character.Dir.Right)
            {
                characterWidth = 1.0f - (float)character.width / 2;
            }
            else if (dir == Character.Dir.Down)
            {
                characterHeight = (float)character.width / 2;
            }
            else if (dir == Character.Dir.Up)
            {
                characterHeight = 1.0f - (float)character.width / 2;
            }
            else
            {
                characterWidth = 0.5f;
            }
        }

        if (character.height > 1)
        {
            if (dir == Character.Dir.Down)// || dir == Character.Dir.Up)
            {
                characterHeight = (float)character.height / 2;
            }
            else if (dir == Character.Dir.Up)
            {
                characterHeight = 1.0f - (float)character.height / 2;
            }
            else if (dir == Character.Dir.Left)
            {
                characterWidth = (float)character.height / 2;
            }
            else if (dir == Character.Dir.Right)
            {
                characterWidth = 1.0f - (float)character.height / 2;
            }
            else
            {
                characterHeight = 0.5f;
            }
        }
    }

    private void SpawnCharacter(Character character, Character.Dir dir)
    {
        int ranX = -100;
        int ranY = -100;
        GridObject gridObject = grid.GetValue(0, 0);

        PlacedCharacter placedCharacter = new PlacedCharacter();

        PlacedCharacter tempCharacter = PlacedCharacter.Create(
            grid.GetWorldPosition(ranX, ranY) + new Vector3(grid.GetCellSize() * characterWidth, 1.6f, grid.GetCellSize() * characterHeight),
            new Vector2Int(ranX, ranY),
            dir,
            character
        );

        if (tempCharacter.enemy)
        {
            ranX = Random.Range(1, width - 1);
            ranY = Random.Range(1, height - 1);

            gridObject = grid.GetValue(ranX, ranY);

            while (gridObject.GetPlacedCharacter() != null)
            {
                ranX = Random.Range(0, width);
                ranY = Random.Range(0, height);

                gridObject = grid.GetValue(ranX, ranY);
            }


            Destroy(tempCharacter);
            placedCharacter = PlacedCharacter.Create(
            grid.GetWorldPosition(ranX, ranY) + new Vector3(grid.GetCellSize() * characterWidth, 1.6f, grid.GetCellSize() * characterHeight),
            new Vector2Int(ranX, ranY),
            dir,
            character
            );

            enemyList.Add(placedCharacter);
            enemyPositionsList.Add(new Vector2Int(ranX, ranY));
            //pathfinding.GetNode(ranX, ranY).SetIsWalkable(false);
        }
        else
        {
            ranX = Random.Range(2, width - 2);
            ranY = Random.Range(6, height - 2);

            gridObject = grid.GetValue(ranX, ranY);

            while (gridObject.GetPlacedCharacter() != null)
            {
                ranX = Random.Range(2, width - 2);
                ranY = Random.Range(6, height - 2);

                gridObject = grid.GetValue(ranX, ranY);
            }

            Destroy(tempCharacter);
            placedCharacter = PlacedCharacter.Create(
            grid.GetWorldPosition(ranX, ranY) + new Vector3(grid.GetCellSize() * characterWidth, 1.6f, grid.GetCellSize() * characterHeight),
            new Vector2Int(ranX, ranY),
            dir,
            character
            );

            allyList.Add(placedCharacter);
            allyPositionsList.Add(new Vector2Int(ranX, ranY));
        }

        List<Vector2Int> gridPositionList = character.GetGridPositionList(new Vector2Int(ranX, ranY), dir);
        SetCharacterOffset(character);

        foreach (Vector2Int gridPosition in gridPositionList)
        {
            grid.GetValue(gridPosition.x, gridPosition.y).SetPlacedCharacter(placedCharacter);
        }

        //pathfinding.GetGrid().GetXY(GetMouseWorldPosition(), out int x, out int y);
        //pathfinding.GetNode(x, y).SetIsWalkable(false);
    }

    private Vector3 GetPointerWorldPosition()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (rayInteractor.TryGetHitInfo(out var hitPosition, out var hitNormal, out _, out _))
        {
            Ray VRRay = Camera.main.ScreenPointToRay(hitPosition);
        }

        //Debug.Log(playerInput.currentControlScheme.ToLower());
        if (Physics.Raycast(mouseRay, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
/*
public class HeatMapGridObject
{
    private const int MIN = 0;
    private const int MAX = 100;

    public int value;
    private int x;
    private int y;
    public Grid<HeatMapGridObject> grid;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int newValue, int width, int height)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            if (value == 40)
            {
                value = 0;
                grid.TriggerGridObjectChanged(x, y);
            }
            else
            {
                value = newValue;
                grid.TriggerGridObjectChanged(x, y);
            }
        }
    }

    public int GetValue()
    {
        return value;
    }

    public float GetValueNormalized()
    {
        return (float)value / MAX;
    }
}

/* could maybe be scavenged for how to move the objects.
private void LateUpdate(){
    Vector3 TargetPosition = GridBuildingSystem.Instance.GetMouseWorldSnappedPosition();
    targetPosition.y = 1f;

    transform.position  = Vector3.Lerp(transform.position, targetPosition, Time.DeltaTime * 15f);
    transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingSyste.Instance.GetPlacedObjectRotation(), Time.DeltaTime * 15f);
}from CodeMonkey's 'Awesome Grid Building System' video*/
//Three to do: turn awitching, enemies move/attack, changing size of movement