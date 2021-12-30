using UnityEngine;

public class ArenaTile : MonoBehaviour 
{
    private bool _empty = true;
    private BallManager tileManager;
    private Ball _tile;

    public Ball Tile 
    {
        get 
        { 
            return _tile; 
        }
        set 
        {
            _tile = value;
            Empty = (value == null);
        }
    }

    public bool Empty 
    { 
        get 
        { 
            return _empty; 
        } 
        private set 
        { 
            _empty = value; 
        } 
    }

    void Start() {
        tileManager = GameObject.FindGameObjectWithTag("BallManager").GetComponent<BallManager>();
    }

    private void OnMouseDown() {
        if (tileManager.selected != null && Empty) tileManager.selected.MoveToPosition(this);
    }
}
