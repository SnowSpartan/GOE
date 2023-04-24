using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private List<GameObject> interactables;
    [SerializeField] private List<GameObject> checkpoints;

    private Vector3 playerPos;
    private GameObject mapPlayer;
    private Queue<Vector3> checkpointLocations;
    private int currentEnemy;
    //private int currentInteractable;
    private List<Character> currentEncounter;
    private List<string> recordedEnemyNames;
    private List<string> recordedInteractableNames;
    private List<bool> removedEnemies;
    private List<bool> removedInteractables;
    private bool gameStarted;
    private bool partyState;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("Player"))
        {
            mapPlayer = GameObject.Find("Player");
            playerPos = mapPlayer.transform.position;
        }

        currentEnemy = -1;
        recordedEnemyNames = new List<string>();
        recordedInteractableNames = new List<string>();
        removedEnemies = new List<bool>();
        removedInteractables = new List<bool>();
        checkpointLocations = new Queue<Vector3>();
        gameStarted = false;
        partyState = true;

        for (int i = 0; i < enemies.Count; i++)
        {
            recordedEnemyNames.Add(enemies[i].name);
            removedEnemies.Add(false);
        }

        for (int i = 0; i < interactables.Count; i++)
        {
            recordedInteractableNames.Add(interactables[i].name);
            removedInteractables.Add(false);
        }

        for(int i = 0; i < checkpoints.Count; i++)
        {
            checkpointLocations.Enqueue(checkpoints[i].transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            gameStarted = true;
        }
    }

    public void SetPlayerPos()
    {
        playerPos = mapPlayer.transform.position;
    }

    public void SetPartyState(bool state)
    {
        partyState = state;
    }

    public void SetCurrentEnemy(int newEnemy)
    {
        currentEnemy = newEnemy;
    }

    public int GetEnemyPositionInList(GameObject enemy)
    {
        return enemies.IndexOf(enemy);
    }

    public void SetEnemies(int enemyToRemove)
    {
        removedEnemies[enemyToRemove] = true;//.Insert(enemyToRemove, true);
    }

    public int GetInteractablePositionInList(GameObject interactable)
    {
        return interactables.IndexOf(interactable);
    }

    public void SetInteractables(int interactableToRemove)
    {
        removedInteractables[interactableToRemove] = true;//.Insert(enemyToRemove, true);
    }

    public List<Character> GetCurrentEncounter()
    {
        return currentEncounter;
    }

    public void SetCurrentEncounter(List<Character> encounter)
    {
        currentEncounter = encounter;
    }

    public void UpdateCheckpoint()
    {
        checkpointLocations.Dequeue();
    }

    public void Load()
    {
        if (GameObject.Find("Player") && gameStarted)
        {
            if(mapPlayer == null)
            {
                mapPlayer = GameObject.Find("Player");
            }
            if (partyState)
            {
                mapPlayer.transform.position = playerPos;
            } else
            {
                mapPlayer.transform.position = checkpointLocations.Peek();
                this.SetPartyState(true);
            }
        }

        if (currentEnemy >= 0 && removedEnemies != null)
        {
            enemies.Clear();

            for(int i = 0; i < removedEnemies.Count; i++)
            {
                enemies.Add(GameObject.Find(recordedEnemyNames[i]));
            }

            this.SetEnemies(currentEnemy);

            for (int i = 0; i < removedEnemies.Count; i++)
            {
                if (removedEnemies[i])
                {
                    //currentEnemy = this.GetCurrentEnemy();
                    enemies[i].SetActive(false);
                }
            }
        }

        if (removedInteractables != null)
        {
            interactables.Clear();

            for (int i = 0; i < removedInteractables.Count; i++)
            {
                interactables.Add(GameObject.Find(recordedInteractableNames[i]));
            }

            for (int i = 0; i < removedInteractables.Count; i++)
            {
                if (removedInteractables[i])
                {
                    //currentEnemy = this.GetCurrentEnemy();
                    interactables[i].SetActive(false);
                }
            }
        }
    }
}
