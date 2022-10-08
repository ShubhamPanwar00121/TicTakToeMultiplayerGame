using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region referances
    public static GameManager Instance { get; private set; }
    [HideInInspector] public PLayerColor playerTurn;
    [HideInInspector] public bool allTilesFilled = false;
    [HideInInspector] public Tile[,] tiles = new Tile[3,3];
    private GameObject board;
    private Tile tilePrefab;
    #endregion

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    public void SetVariables(GameObject board, Tile tilePrefab)
    {
        this.board = board;
        this.tilePrefab = tilePrefab;
    }
    public void SetGameBoard()
    {
        if(board != null && tilePrefab != null)
        {
            for(int i=0;i<tiles.GetLength(0);i++)
            {
                for(int j=0;j<tiles.GetLength(1);j++)
                {
                    Tile InstantiatedTile = Instantiate(tilePrefab, Vector3.one, Quaternion.identity);
                    InstantiatedTile.transform.SetParent(board.transform);
                    InstantiatedTile.transform.SetAsLastSibling();
                    InstantiatedTile.transform.localScale = Vector3.one;
                    InstantiatedTile.SetTileIndexes(i,j);
                    tiles[i, j] = InstantiatedTile;
                }
            }

            if (firebaseManager.instance.playerNumber == 1) playerTurn = PLayerColor.RED;
            else playerTurn = PLayerColor.BLUE;
        }
    }
    public bool CheckBoard(PLayerColor val)
    {

        bool answer = false;
        bool localAnswerD1 = true;
        bool localAnswerD2 = true;
        bool allUsed = true;

        for (int i=0; i< tiles.GetLength(0); i++)
        {
            bool localAnswerR = true;
            bool localAnswerC = true;           

            for (int j=0; j< tiles.GetLength(1); j++)
            {
                if (tiles[i, j].playerColor != val)
                    localAnswerR = false;
                if (tiles[j, i].playerColor != val)
                    localAnswerC = false;
                if (tiles[i, j].playerColor == PLayerColor.NONE)
                    allUsed = false;

                if (i==j)
                {
                    if (tiles[i, j].playerColor != val)
                        localAnswerD1 = false;
                }

                if ((i+j) == (tiles.GetLength(0)-1))
                {
                    if (tiles[i, j].playerColor != val)
                        localAnswerD2 = false;
                }
            }
            if ( localAnswerR || localAnswerC )
            {
                answer = true;
                break;
            }
        }
        if (!answer && (localAnswerD1 || localAnswerD2)) answer = true;
        allTilesFilled = allUsed;
        return answer;
    }
    public void changeTurns()
    {
        if (playerTurn == PLayerColor.RED)
            playerTurn = PLayerColor.BLUE;
        else
            playerTurn = PLayerColor.RED;
    }
    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

}
