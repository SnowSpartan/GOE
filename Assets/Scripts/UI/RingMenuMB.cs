using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RingMenuMB : MonoBehaviour
{
    public RingMenu data;
    public RingMenuCakePiece ringCakePiecePrefab;
    public float gapWidthDegree = 1f;
    //public Action<string> callback;
    protected RingMenuCakePiece[] pieces;
    protected RingMenuMB parent;
    public string path;

    private int activeElement;

    void Start()
    {
        var stepLength = 360f / data.Elements.Length;
        var iconDist = Vector3.Distance(ringCakePiecePrefab.icon.transform.position, ringCakePiecePrefab.cakePiece.transform.position);

        pieces = new RingMenuCakePiece[data.Elements.Length];

        for(int i = 0; i < data.Elements.Length; i++)
        {
            pieces[i] = Instantiate(ringCakePiecePrefab, transform);

            pieces[i].transform.localPosition = Vector3.zero;
            pieces[i].transform.localRotation = Quaternion.identity;

            pieces[i].cakePiece.fillAmount = 1f / data.Elements.Length - gapWidthDegree / 360f;
            pieces[i].transform.localPosition = Vector3.zero;
            pieces[i].transform.localRotation = Quaternion.Euler(0, 0, stepLength / 2f + gapWidthDegree / 2f + i * stepLength);

            //pieces[i].icon.transform.localPosition = pieces[i].cakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;// * Vector3.up * 5f;
            pieces[i].icon.sprite = data.Elements[i].icon;
        }
    }

    void Update()//Might want to get this off update, so there isn't two scripts activating every click
    {
        var stepLength = 360f / data.Elements.Length;
        var mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2), Vector3.forward) + stepLength / 2f);
        if (gameObject.activeSelf)
        {
            activeElement = (int)(mouseAngle / stepLength);
        }

        for (int i = 0; i < data.Elements.Length; i++) {
            if(i == activeElement)
            {
                pieces[i].cakePiece.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                pieces[i].cakePiece.color = new Color(1f, 1f, 1f, .75f);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Despawn();
        }
    }

    private float NormalizeAngle(float a) => (a + 540f) % 360f;

    public void Spawn()
    {
        gameObject.SetActive(true);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    public string GetName()
    {
        return data.Elements[activeElement].name;
    }

    public bool activeSelf()
    {
        return gameObject.activeSelf;
    }
}
