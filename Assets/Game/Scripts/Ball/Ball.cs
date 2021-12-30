using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Ball : MonoBehaviour {
    [SerializeField] private Material matUnselect;
    [SerializeField] private Material matSelect;
    private MeshRenderer mesh;
    private bool _selected = false;
    private bool _movement = false;
    private bool toRemove = false;
    private Color _color;
    private BallManager manager;
    private NavMeshAgent navMesh;
    private NavMeshObstacle navObstacle;
    private ArenaTile currentTile;
    private Arena arena;
    private Animation anim;
    private AudioSource audioSource;
    public AudioClip clip;

    public bool Selected {
        get 
        { 
            return _selected; 
        }
        set 
        {
            if (value) 
            {
                if (manager.selected != null) manager.selected.Selected = false;
                mesh.material = matSelect;
                manager.selected = this;
            }
            else 
            {
                manager.selected = null;
                mesh.material = matUnselect;
            }

            _selected = value;
            if(!toRemove) StartCoroutine(toggleNavigation());
            mesh.material.color = color;
        }
    }

    public bool Movement 
    {
        get 
        { 
            return _movement; 
        }
        private set 
        { 
            _movement = value; 
        }
    }

    public Color color {
        get 
        { 
            return _color; 
        }
        set 
        {
            _color = value;
            if (mesh != null) mesh.material.color = value;
            else 
            {
                mesh = GetComponent<MeshRenderer>();
                color = value;
            }
        }
    }

    void Start() 
    {
        audioSource = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        navMesh = GetComponent<NavMeshAgent>();
        navObstacle = GetComponent<NavMeshObstacle>();
        manager = transform.parent.GetComponent<BallManager>();
        arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Arena>();
        anim = GetComponent<Animation>();
    }

    void LateUpdate() 
    {
        if(Movement) 
        {
           
            if (navMesh.remainingDistance == Mathf.Infinity) transform.rotation = new Quaternion(); //Freeze rotation while moving
            else if(navMesh.remainingDistance == 0) 
            {
                Debug.Log("Ball on position, checking points", gameObject);
                arena.CheckPoints();
                
                Movement = false;
                Selected = false;

            }
        }
    }

    void OnMouseDown() 
    {
        if (Selected && !Movement) 
            Selected = false;
        else 
        {
            if(manager.selected == null || !manager.selected.Movement) 
                Selected = true;
        }
    }

    public void Initialize(ArenaTile arTile, Color col) 
    {
        transform.position = arTile.transform.position;
        color = col;
        currentTile = arTile;
        arTile.Tile = this;

        anim = GetComponent<Animation>();
        transform.localScale = new Vector3(0, transform.localScale.y, 0);   //Set scale to 0 before start animation
        anim.Play("Ball New");
    }

    //WIP
    //public void InitializeGhost(ArenaTile arTile)
    //{
    //    transform.position = arTile.transform.position;
    //    color = Color.white;
    //    currentTile = arTile;
    //    arTile.Tile = this;

    //    anim = GetComponent<Animation>();
    //    transform.localScale = new Vector3(0, 2, 0);
    //    anim.Play("Ball New");
    //}

    public void MoveToPosition(ArenaTile tile) 
    {
        Vector3 pos = tile.transform.position;
        NavMeshPath path = new NavMeshPath();
        navMesh.CalculatePath(pos, path);

        if(path.status == NavMeshPathStatus.PathComplete) 
        {
            navMesh.SetPath(path);

            if (currentTile != null) currentTile.Tile = null;
            currentTile = tile;
            tile.Tile = this;
            Movement = true;
        }
        else 
        {
            Debug.Log("Cannot reach destination.", gameObject);
            Selected = false;
        }
    }

    public void Remove() 
    {
        
        Debug.Log("Remove tile", gameObject);

        toRemove = true;
        Selected = false;
        currentTile.Tile = null;

        anim.Play("Ball Remove");
        
    }

    public void OnStart()
    {
        audioSource.PlayOneShot(clip, 1);
    }
    public void Destroy() 
    {
        
        Destroy(gameObject); 
    }  //Used in Ball Remove animation event

    private IEnumerator toggleNavigation() 
    {    //Toggling between Nav Mesh Agent and Nav Obstacle must be delayed because of bug (switch move/stop)
        float waitTime = 0.005f;

        if (navMesh.enabled) 
        {
            navMesh.enabled = false;
            yield return new WaitForSeconds(waitTime);
            navObstacle.enabled = true;
        }
        else 
        {
            navObstacle.enabled = false;
            yield return new WaitForSeconds(waitTime);
            navMesh.enabled = true;
        }
    }
}
