using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI angleText;
    public static GameManager Instance { get; private set; }

    public LogManager logManager;

    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4;
    [SerializeField] private Transform gameHolder;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private RectTransform puzzleSpace;

    [Header("UI Elements")]
    [SerializeField] private List<Texture2D> imageTextures;
    [SerializeField] private Transform levelSelectPanel;
    [SerializeField] private Image levelSelectPrefab;
    [SerializeField] private Transform draggingPiece; 


    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    private int currentImageIndex = 0;

    void Start() {
        //foreach (Texture2D texture in imageTextures) {
        //    StartGame(texture);
        //}
        StartGame(imageTextures[currentImageIndex]);
        currentImageIndex++;
    }

    public void StartGame(Texture2D jigsawTexture) {
        pieces = new List<Transform>();

        dimensions = GetDimensions(jigsawTexture, difficulty);

        CreateJigsawPieces(jigsawTexture);

        Scatter();

        UpdateBorder();
    }

    Vector2Int GetDimensions(Texture2D jigsawTexture, int difficulty) {
        Vector2Int dimensions = Vector2Int.zero;
        if (jigsawTexture.width < jigsawTexture.height) {
            dimensions.x = difficulty;
            dimensions.y = (difficulty * jigsawTexture.height) / jigsawTexture.width;
        } else {
            dimensions.x = (difficulty * jigsawTexture.width) / jigsawTexture.height;
            dimensions.y = difficulty;
        }
        return dimensions;
    }

    void CreateJigsawPieces(Texture2D jigsawTexture) {
        height = 1f / dimensions.y;
        float aspect = (float)jigsawTexture.width / jigsawTexture.height;
        width = aspect / dimensions.x;

        for (int row = 0; row < dimensions.y; row++) {
            for (int col = 0; col < dimensions.x; col++) {
                Transform piece = Instantiate(piecePrefab, gameHolder);
                piece.localPosition = new Vector3(
                    (-width * dimensions.x / 2) + (width * col) + (width / 2),
                    (-height * dimensions.y / 2) + (height * row) + (height / 2),
                    0);
            
                piece.localScale = new Vector3(width, height, 0.01f);


                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                Vector2[] uv = mesh.uv;

                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;

                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                for (int i = 4; i < uv.Length; i++) {
                    uv[i] = Vector2.zero;
                }

                mesh.uv = uv;
                piece.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", jigsawTexture);
            }
        }
    }

    private void Scatter() {
        Rect panelRect = puzzleSpace.rect;

        foreach (Transform piece in pieces) {
            float x = Random.Range(panelRect.xMin, panelRect.xMax);
            float y = Random.Range(panelRect.yMin, panelRect.yMax);
            piece.localPosition = new Vector3(x, y, 0);

            PuzzlePiece puzzlePiece = piece.GetComponent<PuzzlePiece>();
            if (puzzlePiece != null) {
                puzzlePiece.SavePosition();
            }
        }
    }

    private void UpdateBorder() {
        LineRenderer lineRenderer = gameHolder.GetComponent<LineRenderer>();

        float halfWidth = (width * dimensions.x) / 2f;
        float halfHeight = (height * dimensions.y) / 2f;

        float borderZ = 0f;

        lineRenderer.SetPosition(0, new Vector3(-halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, halfHeight, borderZ));
        lineRenderer.SetPosition(2, new Vector3(halfWidth, -halfHeight, borderZ));
        lineRenderer.SetPosition(3, new Vector3(-halfWidth, -halfHeight, borderZ));

        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f; 

        lineRenderer.enabled = true;      
    }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void SetDraggingPiece(Transform piece) {
        draggingPiece = piece;
    }
    
    public void ClearDraggingPiece() {
        draggingPiece = null;
    }

    private void ClearPuzzlePieces() {
        if (pieces != null) {
            foreach (Transform piece in pieces) {
                Destroy(piece.gameObject);
            }
            pieces.Clear();
        }
    }

    public void CheckPlacement(Transform piece) {

        Debug.Log($"CheckPlacement {piece.name}");

        int pieceIndex = pieces.IndexOf(piece);
        if (pieceIndex == -1) return;

        int col = pieceIndex % dimensions.x;
        int row = pieceIndex / dimensions.x;

        Vector3 targetPosition = new Vector3(
            ((width * dimensions.x / 2) - (width * col) - (width / 2)),
            (-height * dimensions.y / 2) + (height * row) + (height / 2),
            0
        );

        Quaternion targetRotation = Quaternion.Euler(270, 0, 90);

        Debug.Log($"targetPosition: {targetPosition}, localPosition: {piece.localPosition}");

        angleText.text = $"Rotation: {Quaternion.Angle(piece.rotation, targetRotation)}°";

        if (Vector3.Distance(piece.localPosition, targetPosition) < (width * 0.75) && Quaternion.Angle(piece.rotation, targetRotation) < 45){

            Debug.Log($"Puzzle-Teil {piece.name} richtig platziert!");
           
            // piece.localPosition = targetPosition;

            // piece.transform.rotation = targetRotation;
            PuzzlePiece puzzlePiece = piece.GetComponent<PuzzlePiece>();
            puzzlePiece.isAnimated = true;
            StartCoroutine(AnimatePiecePlacement(piece, targetPosition, targetRotation, puzzlePiece.GetPieceThickness()));
            puzzlePiece.isAnimated = false;

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = piece.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null) {
                grabInteractable.enabled = false;
            }

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.isKinematic = true;
            }

            //logManager.AddPuzzleLog("puzzle_piece_set"); TODO: ENABLE HERE

            CheckPuzzleCompletion();
        }
    }

    private IEnumerator AnimatePiecePlacement(Transform piece, Vector3 targetPosition, Quaternion targetRotation, float pieceThickness) {
        float duration = 0.05f; 
        float elapsedTime = 0f;

        Vector3 startPosition = targetPosition - new Vector3(0, 0, pieceThickness * 2); 

        while (elapsedTime < duration)
        {
            piece.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            piece.rotation = Quaternion.Lerp(piece.rotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;  
        }

        piece.localPosition = targetPosition;
        piece.rotation = targetRotation;
    }


    private void CheckPuzzleCompletion() {
        angleText.text = "CheckPuzzleCompletion!";
        foreach (Transform piece in pieces) {
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = piece.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null && grabInteractable.enabled) {
                return;
            }
        }

        Debug.Log("Puzzle komplett gelöst!");
        //logManager.AddPuzzleLog("puzzle_finished"); //TODO: ENABLE HERE

        if(currentImageIndex + 1 > imageTextures.Count)
        {
            currentImageIndex = 0;
        }
        ClearPuzzlePieces();
        StartGame(imageTextures[currentImageIndex]);
        currentImageIndex++;
    }


}