using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flipper : OOB_Objects
{
    [SerializeField]
    private float bounciness = 1;

    public float Bounciness
    {
        get { return bounciness; }
    }
}
