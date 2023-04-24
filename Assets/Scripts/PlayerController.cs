using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private GameObject battleCamera;
    //[SerializeField] private GameObject obelisk;
    //[SerializeField] private GameObject obelisk2;//make into an array
    //[SerializeField] private GameObject ally;
    //[SerializeField] private GameObject wolf;
    [SerializeField] private List<Character> party;


    private List<Character> newEncounter;
    private float speed = 4.0f;
    private float turnSpeed = 45.0f;
    private float horizontalInput;
    private float verticalInput;
    private float pitch;
    //private float mapActive = true;
    private Rigidbody playerRb;
    private GameObject mapPlayer;
    private GameObject gameManager;
    private int checkpointFlag = 0;
    private PlacedCharacter placedCharacter;
    public AudioClip encounter;
    public AudioClip save;
    public AudioClip newPartyMember;

    // Start is called before the first frame update
    void Start()
    {
        newEncounter = new List<Character>();

        playerRb = GetComponent<Rigidbody>();
        mapPlayer = GameObject.Find("Player");
        if(GameObject.Find("GameManager") != null){
            gameManager = GameObject.Find("GameManager");
            gameManager.GetComponent<GameManager>().Load();
        }
    }

    private void OnMove(InputValue movement)
    {
        Vector2 movementVector = movement.Get<Vector2>();
        horizontalInput = movementVector.x;
        verticalInput = movementVector.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        //verticalInput = Input.GetAxis("Vertical");

        //if (mapActive)
        //{
            transform.Translate(Vector3.forward * Time.deltaTime * speed * verticalInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);
        //}
    }

    void OnCollisionEnter(Collision theCollision)
    {
        int currentEnemy = gameManager.GetComponent<GameManager>().GetEnemyPositionInList(theCollision.gameObject);
        int currentInteractable = gameManager.GetComponent<GameManager>().GetInteractablePositionInList(theCollision.gameObject);

        if ((PlacedCharacter)theCollision.gameObject.GetComponent(typeof(PlacedCharacter)) != null)
        {
            PlacedCharacter placedCharacter = (PlacedCharacter)theCollision.gameObject.GetComponent(typeof(PlacedCharacter));
            if (placedCharacter.GetAlliegence()) // If A collides with B
            {
                newEncounter = theCollision.gameObject.GetComponent<EnemyMovement>().GetEnemies();
                for(int i = 0; i < party.Count; i++)
                {
                    newEncounter.Add(party[i]);
                }
                theCollision.gameObject.SetActive(false);
                //battleCamera.SetActive(true);
                //mapPlayer.SetActive(false);
                //mapActive = false;
                //battleCamera.transform.position = new Vector3(-55, 4, -45);
                //battleCamera.GetComponent<AudioSource>().PlayOneShot(encounter);
                //DontDestroyOnLoad(this.mapPlayer);
                gameManager.GetComponent<GameManager>().SetCurrentEnemy(currentEnemy);
                gameManager.GetComponent<GameManager>().SetPlayerPos();
                gameManager.GetComponent<GameManager>().SetCurrentEncounter(newEncounter);

                newEncounter = new List<Character>();

                DontDestroyOnLoad(this.gameManager);
                SceneManager.LoadScene("Test");
                //DontDestroyOnLoad(this.FPS);
                //SceneManager.LoadScene("Test", LoadSceneMode.Additive);
            }
            else if (theCollision.gameObject.name == "Distance Guardian")
            {
                mapPlayer.GetComponent<AudioSource>().PlayOneShot(newPartyMember);
                theCollision.gameObject.SetActive(false);
                gameManager.GetComponent<GameManager>().SetInteractables(currentInteractable);

                //Debug.Log(party.Count);
            }
        }
        else if(theCollision.gameObject.name == "Obelisk (1)" && checkpointFlag == 0)
        {
            pitch = Random.Range(.9f, 1.1f);
            theCollision.gameObject.GetComponent<AudioSource>().pitch = pitch;
            theCollision.gameObject.GetComponent<AudioSource>().PlayOneShot(save);
            gameManager.GetComponent<GameManager>().UpdateCheckpoint();
            //obelisk2.GetComponent<AudioSource>().PlayOneShot(save);
        }

        else if (theCollision.gameObject.name == "Obelisk")
        {
            //theCollision.gameObject.GetComponent<AudioSource>().PlayOneShot(save);
            //gameManager.GetComponent<GameManager>().UpdateCheckpoint();
            //obelisk2.GetComponent<AudioSource>().PlayOneShot(save);
            DontDestroyOnLoad(this.gameManager);
            SceneManager.LoadScene("Camp");
        }
        //battleCamera.GetComponent<AudioSource>().PlayOneShot(encounter);
    }
}
