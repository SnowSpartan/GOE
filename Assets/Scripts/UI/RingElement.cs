using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RingElement", menuName = "RingMenu/Element", order = 2)]
public class RingElement : ScriptableObject
{
    public string name;
    public Sprite icon;
    public RingMenu nextRing;
}
