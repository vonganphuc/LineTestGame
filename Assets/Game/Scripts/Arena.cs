using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {
    [SerializeField] private int requiredTilesInLine = 3;
    private enum TilesCompareState { EMPTY, DIFFERENT, SAME }   //Used to prettier comparison in looking for points
    private ArenaTile[,] _tile;
    private List<ArenaTile> tileList;   //Used to found empty tiles
    private int maxX, maxY;
    private Game game;
    
    public ArenaTile[,] tile 
    { 
        get { return _tile; } 
        private set { _tile = value; } 
    }

	void Awake () 
    {
        maxX = transform.GetChild(0).childCount;
        maxY = transform.childCount;
        Debug.Log("maxX, maxY: " + maxX + maxY);
        tile = new ArenaTile[maxX, maxY];
        tileList = new List<ArenaTile>();
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        for(int y = 0; y < maxY; ++y) 
        {
            for(int x = 0; x < maxX; ++x) 
            {
                tile[x, y] = transform.GetChild(y).GetChild(x).GetComponent<ArenaTile>();
                tileList.Add(tile[x, y]);
            }
        }
	}

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.C)) CheckPoints();
    }

    public List<ArenaTile> GetEmptyTiles() 
    {
        Debug.Log(string.Format("Found {0} empty tiles.", tileList.FindAll(b => b.Empty).Count));

        return tileList.FindAll(b => b.Empty);
    }

    public void RemoveAllTiles() 
    {
        Debug.Log("Remove all tiles on arena");

        foreach(var obj in tileList.FindAll(b => !b.Empty)) 
        {
            obj.Tile.Remove();
        }
    }

    public void CheckPoints(bool spawnCheck = false) 
    {  //spawnCheck - avoid spawning new tiles without player move (from tile spawner)
        bool pointsRow = CheckPointsRow();
        bool pointsCol = CheckPointsColumn();
        bool pointsDiagonal = CheckPointsDiagonal();

        if (!spawnCheck && !pointsRow && !pointsCol && !pointsDiagonal)
        {
            game.Spawner.Spawn();
        }
    }

    
    private bool CheckPointsRow() 
    {
        bool achievedPoints = false;

        for (int y = 0; y < maxY; ++y)  //kiểm tra từ hàng 1 đến max
        {
            int sameColor = 1, start = 0;  //chung màu = 1, bắt đầu = 0
            
            for (int x = 1; x < maxX; ++x)  //kiểm tra từ cột 1 đến max trước
            {  
                TilesCompareState compState = CompareTiles(tile[x, y], tile[x - 1, y]); //so sánh tile [x,y] với tile ngay bên phải của nó, [1,0] vs [0,0]

                if (compState == TilesCompareState.SAME) //nếu trùng thì thêm 1 đơn vị chung màu 
                {
                    ++sameColor;

                    if(x == maxX - 1 && sameColor >= requiredTilesInLine) //nếu là cột kế bên max, và số đơn vị chung màu lớn hơn hoặc bằng yêu cầu
                    {
                        achievedPoints = true; //nhận được điểm
                        RemoveRow(y, start, x); // thực thi lệnh xóa ô trên hàng y từ cột start đến cột x 
                    }
                }
                else //nếu không trùng màu thì
                {
                    if (sameColor >= requiredTilesInLine) // kiểm tra nếu điểm trùng màu >= yêu cầu
                    {
                        achievedPoints = true; // nhận điểm
                        RemoveRow(y, start, x - 1); //xóa ô
                    }

                    sameColor = 1; //điểm trùng màu = 1
                    start = x; // bắt đầu cột hiện tại
                }
            }
        }

        return achievedPoints;
    }
    private void RemoveRow(int row, int start, int end)// gia tri hang, bat dau, ket thuc
    {
        Debug.Log(string.Format("Remove row: {0}, start: {1}, end: {2}, total tiles: {3}", row + 1, start + 1, end + 1, end - start + 1));

        //game.AddPoints(Mathf.Abs(requiredTilesInLine - (end - start + 1))); // tinh diem, so tile can co - so tile cung hang + 1

        for (int i = start; i <= end; ++i)
        {
            game.AddPoints(1);
            tile[i, row].Tile.Remove();//xóa các ô từ cột start đến cột end trên hàng row
        }   
    }

    
   

    private void RemoveDiagonal1(int startX, int endX, int startY) // giá trị cột bắt đầu , cột kết thúc, hàng bắt đầu
    {
        //Debug.Log(string.Format("Remove column: {0}, start: {1}, end: {2}, total tiles: {3}", col + 1, start + 1, end + 1, end - start + 1));

        //game.AddPoints(Mathf.Abs(requiredTilesInLine - (endX - startX + 1)));

        int j = startY;
        for (int i = startX; i <= endX; ++i)
        {
            Debug.Log("tile [" + i + "," + j + "]");
            game.AddPoints(1);
            tile[i, j].Tile.Remove(); //xóa các ô từ trái sang phải có số hàng từ StartY tăng dần \
            j++;
        }

    }
   

    private void RemoveDiagonal2(int startX, int endX, int startY) // giá trị cột bắt đầu , cột kết thúc, hàng bắt đầu
    {
        //Debug.Log(string.Format("Remove column: {0}, start: {1}, end: {2}, total tiles: {3}", col + 1, start + 1, end + 1, end - start + 1));

        //game.AddPoints(Mathf.Abs(requiredTilesInLine - (endX - startX + 1)));

        int j = startY;
        for (int i = startX; i <= endX; ++i)
        {
            Debug.Log("tile ["+ i +","+ j+"]");
            game.AddPoints(1);
            tile[i, j].Tile.Remove(); //xóa các ô từ phải sang trái có số hàng từ StartY tăng dần /
            j--;
        }

    }

    private bool CheckPointsDiagonal()
    {
        bool achievedPoints = false;

        for (int x = 2; x < maxX - 2; x++) //kiếm tra từ cột 1 đến maxX - 1
        {

            for (int y = 2; y < maxY - 2; y++) //kiếm tra từ hàng 1 đến maxY - 1, khởi đầu từ ô [1,1]
            {
                TilesCompareState compState1 = CompareTiles(tile[x, y], tile[x - 1, y - 1]); //trái trên
                TilesCompareState compState2 = CompareTiles(tile[x, y], tile[x + 1, y + 1]); //phải dưới

                TilesCompareState compstate3 = CompareTiles(tile[x, y], tile[x + 1, y - 1]); //phải trên
                TilesCompareState compstate4 = CompareTiles(tile[x, y], tile[x - 1, y + 1]); //trái dưới


                if (compState1 == TilesCompareState.SAME && compState2 == TilesCompareState.SAME)
                {
                 
                    TilesCompareState compState5 = CompareTiles(tile[x, y], tile[x - 2, y - 2]); 
                    TilesCompareState compState6 = CompareTiles(tile[x, y], tile[x + 2, y + 2]);
                    if (compState5 == TilesCompareState.SAME && compState6 == TilesCompareState.SAME)
                    {
                        achievedPoints = true; //nhận được điểm
                        RemoveDiagonal1(x - 2, x + 2, y - 2); // thực thi lệnh xóa ô  \
                    }
                }
                else if (compstate3 == TilesCompareState.SAME && compstate4 == TilesCompareState.SAME)
                {
                    TilesCompareState compState7 = CompareTiles(tile[x, y], tile[x + 2, y - 2]);
                    TilesCompareState compState8 = CompareTiles(tile[x, y], tile[x - 2, y + 2]);
                    if (compState7 == TilesCompareState.SAME && compState8 == TilesCompareState.SAME)
                    {
                        achievedPoints = true; //nhận được điểm
                        RemoveDiagonal2(x - 2, x + 2, y + 2); // thực thi lệnh xóa ô  /
                    }
                }
            }
        }               
        return achievedPoints;
    }

    private bool CheckPointsColumn() 
    {
        bool achievedPoints = false;

        for (int x = 0; x < maxX; ++x) 
        {
            int sameColor = 1, start = 0;

            for (int y = 1; y < maxY; ++y) 
            {
                TilesCompareState compState = CompareTiles(tile[x, y], tile[x, y - 1]);

                if (compState == TilesCompareState.SAME) 
                {
                    ++sameColor;

                    if (y == maxY - 1 && sameColor >= requiredTilesInLine) 
                    {
                        achievedPoints = true;
                        RemoveColumn(x, start, y);
                    }
                }
                else {
                    if (sameColor >= requiredTilesInLine) 
                    {
                        achievedPoints = true;
                        RemoveColumn(x, start, y - 1);
                    }

                    sameColor = 1;
                    start = y;
                }
            }
        }

        return achievedPoints;
    }

    private TilesCompareState CompareTiles(ArenaTile A, ArenaTile B) 
    {
        if (A.Empty || B.Empty) 
            return TilesCompareState.EMPTY;
        else if (A.Tile.color == B.Tile.color) 
            return TilesCompareState.SAME;
        else 
            return TilesCompareState.DIFFERENT;
    }

    

    private void RemoveColumn(int col, int start, int end) 
    {
        Debug.Log(string.Format("Remove column: {0}, start: {1}, end: {2}, total tiles: {3}", col + 1, start + 1, end + 1, end - start + 1));

        //game.AddPoints(Mathf.Abs(requiredTilesInLine - (end - start + 1)));

        for (int i = start; i <= end; ++i)
        {
            game.AddPoints(1);
            tile[col, i].Tile.Remove();
        }
           
    }
    
}
