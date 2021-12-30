using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour 
{
    private Ball _selected;

    public Ball selected 
    { 
        get 
        { 
            return _selected; 
        } 
        set 
        { 
            _selected = value; 
        } 
    }
}
