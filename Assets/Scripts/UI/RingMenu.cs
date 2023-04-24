using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "RingMenu/Ring", order = 1)]
public class RingMenu : ScriptableObject
{
    public RingElement[] Elements;
}
