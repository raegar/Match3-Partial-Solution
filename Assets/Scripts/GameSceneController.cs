using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public int BoardWidth = 6;
    public int BoardHeight = 6;
    public float PuzzlePieceSpacing = 1.5f;


    public Camera GameCamera;
    public GameObject PuzzlePiecePrefab;
    public Transform Level;

    private PuzzlePiece[,] board;
    private PuzzlePiece selectedPuzzlePiece;

    private int score;
    private bool gameOver;
   

    // Start is called before the first frame update
    void Start()
    {
        BuildBoard();
    }

    private void BuildBoard()
    {
        board = new PuzzlePiece[BoardWidth, BoardHeight];

        for(int y = 0; y < BoardHeight; y++)
        {
            for(int x = 0; x < BoardWidth; x++)
            {
                GameObject puzzlePieceInstance = Instantiate(PuzzlePiecePrefab);
                puzzlePieceInstance.transform.SetParent(Level);
                puzzlePieceInstance.transform.localPosition = new Vector3(
                    (-BoardWidth * PuzzlePieceSpacing)/2 + (PuzzlePieceSpacing / 2) + x * PuzzlePieceSpacing,
                    (-BoardHeight * PuzzlePieceSpacing)/2 + (PuzzlePieceSpacing / 2) + y * PuzzlePieceSpacing,
                    0f
                );

                PuzzlePiece puzzlePiece = puzzlePieceInstance.GetComponent<PuzzlePiece>();

                puzzlePiece.BoardLocation = new Vector2(x, y);
                board[x, y] = puzzlePiece;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {

        //reset if R is pressed
        //return if game over
        
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = GameCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if(hitCollider != null && hitCollider.gameObject.GetComponent<PuzzlePiece>() != null)
            {
                PuzzlePiece hitPuzzlePiece = hitCollider.gameObject.GetComponent<PuzzlePiece>();

                if(selectedPuzzlePiece == null)
                {
                    selectedPuzzlePiece = hitPuzzlePiece;

                    iTween.ScaleTo(selectedPuzzlePiece.gameObject, iTween.Hash(
                        "scale", Vector3.one * 0.8f,
                        "time", 0.2f
                    ));
                }
                else
                {
                    if(selectedPuzzlePiece == hitPuzzlePiece)
                    {
                        iTween.ScaleTo(selectedPuzzlePiece.gameObject, iTween.Hash(
                            "scale", Vector3.one,
                            "time", 0.2f
                        ));
                        selectedPuzzlePiece = null;
                    }
                    else if(hitPuzzlePiece.IsNeighbour(selectedPuzzlePiece))
                    {
                        StartCoroutine(AttemptMatchRoutine(selectedPuzzlePiece, hitPuzzlePiece));
                        selectedPuzzlePiece = null;
                    }
                }
            }
        }
    }

    private IEnumerator AttemptMatchRoutine(PuzzlePiece puzzlePiece1, PuzzlePiece puzzlePiece2)
    {
        iTween.Stop(puzzlePiece1.gameObject);
        iTween.Stop(puzzlePiece2.gameObject);
        
        puzzlePiece1.transform.localScale = Vector3.one;
        puzzlePiece2.transform.localScale = Vector3.one;

        Vector2 boardLocation1 = puzzlePiece1.BoardLocation;
        Vector2 boardLocation2 = puzzlePiece2.BoardLocation;

        Vector3 position1 = puzzlePiece1.transform.position;
        Vector3 position2 = puzzlePiece2.transform.position;
        Debug.Log("Start animating");
        iTween.MoveTo(puzzlePiece1.gameObject, iTween.Hash(
            "position", position2,
            "time", 0.5f
        ));

        iTween.MoveTo(puzzlePiece2.gameObject, iTween.Hash(
            "position", position1,
            "time", 0.5f
        ));

        puzzlePiece1.BoardLocation = boardLocation2;
        puzzlePiece2.BoardLocation = boardLocation1;

        board[(int)puzzlePiece1.BoardLocation.x, (int)puzzlePiece1.BoardLocation.y] = puzzlePiece1;
        board[(int)puzzlePiece2.BoardLocation.x, (int)puzzlePiece2.BoardLocation.y] = puzzlePiece2;

        yield return new WaitForSeconds(0.5f);

        List<PuzzlePiece> matchingPieces = CheckMatch(puzzlePiece1);

        if(matchingPieces.Count == 0)
        {
            matchingPieces = CheckMatch(puzzlePiece2);
        }

        if(matchingPieces.Count < 3)
        {
            iTween.MoveTo(puzzlePiece1.gameObject, iTween.Hash(
                "position", position1,
                "time", 0.5f
            ));

            iTween.MoveTo(puzzlePiece2.gameObject, iTween.Hash(
                "position", position2,
                "time", 0.5f
            ));

            puzzlePiece1.BoardLocation = boardLocation1;
            puzzlePiece2.BoardLocation = boardLocation2;
            
            board[(int)puzzlePiece1.BoardLocation.x, (int)puzzlePiece1.BoardLocation.y] = puzzlePiece1;
            board[(int)puzzlePiece2.BoardLocation.x, (int)puzzlePiece2.BoardLocation.y] = puzzlePiece2;

            yield return new WaitForSeconds(0.5f);

            //CheckGameOver();
        }
        else
        {
            foreach(PuzzlePiece puzzlePiece in matchingPieces)
            {
                puzzlePiece.Destroyed = true;
                score += 100;
                iTween.ScaleTo(selectedPuzzlePiece.gameObject, iTween.Hash(
                    "scale", Vector3.zero,
                    "time", 0.3f
                ));
            }
            yield return new WaitForSeconds(0.3f);

            //Drop puzzle pieces
            //Add new puzzle pieces

            //wait for 1 second

            //Check for game over

        }

    }

    private List<PuzzlePiece> CheckMatch(PuzzlePiece puzzlePiece)
    {
        return new List<PuzzlePiece>();
    }
}
