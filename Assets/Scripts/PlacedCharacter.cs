using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlacedCharacter : MonoBehaviour
{
    private Character character;
    private Character.Dir dir;
    private Vector2Int origin;
    public string nameString;
    public float speed;
    public float health;
    private float instanceHealth;
    public float armor;
    public float damage;
    public bool enemy;
    public bool canMove;

    void Awake()
    {
        instanceHealth = health;
        canMove = true;
    }

    public static PlacedCharacter Create(Vector3 worldPosition, Vector2Int origin, Character.Dir dir, Character character)
    {
        Transform placedCharacterTransform = Instantiate(character.prefab, worldPosition, Quaternion.Euler(0, character.GetRotationAngle(dir), 0));
        PlacedCharacter placedCharacter = placedCharacterTransform.GetComponent<PlacedCharacter>();
        placedCharacter.character = character;
        placedCharacter.origin = origin;
        placedCharacter.dir = dir;
        placedCharacter.instanceHealth = placedCharacter.health;

        return placedCharacter;
    }

    public GameObject GetGameObject()
    {
        return transform.gameObject;
    }

    public string GetName()
    {
        return nameString;
    }

    public Character GetCharacter()
    {
        return character;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return character.GetGridPositionList(origin, dir);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public bool GetAlliegence()
    {
        return enemy;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetHealth()
    {
        return instanceHealth;
    }

    public void SetHealth(float hp)
    {
        instanceHealth = hp;
    }

    //public void RestoreHealth()
    //{
        //instanceHealth = health;
    //}

    public float ChangeHealth(float dmg)
    {
        if (armor < dmg)
        {
            Debug.Log("Old: " + instanceHealth);
            instanceHealth -= (dmg - armor);
            Debug.Log("  New: " + instanceHealth);
            return instanceHealth;
        }
        else
        {
            return instanceHealth;
        }
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public Vector2Int GetOrigin()
    {
        return origin;
    }

    public void SetCanMove(bool move)
    {
        canMove = move;
        if (move)
        {
            this.GetCharacter().GetPrefab().GetComponent<VisualEffect>().Play();
            Debug.Log("Play");
        }
        else
        {
            Debug.Log("Stop");
            this.GetCharacter().GetPrefab().GetComponent<VisualEffect>().Stop();
        }
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        //A* video
    }

    private void HandleMovement()
    {

    }
}