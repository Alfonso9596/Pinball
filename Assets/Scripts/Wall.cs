using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : OOB_Objects
{
    [SerializeField]
    private float bounciness = 0;

    public float Bounciness
    {
        get { return bounciness; }
    }
}
