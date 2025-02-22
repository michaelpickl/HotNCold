using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class GameManager : MonoBehaviour
{
    //public TextMeshProUGUI angleText;
    public static GameManager Instance { get; private set; }

    public LogManager logManager;

    public ParticleSystem finishParticles;
    public GameObject frame;

    [Header("Game Elements")]
    [Range(2, 6)]
    [SerializeField] private int difficulty = 4;
    [SerializeField] private Transform gameHolder;
    [SerializeField] private Transform piecePrefab;
    [SerializeField] private RectTransform puzzleSpace;

    [Header("UI Elements")]
    [SerializeField] private List<Texture2D> imageTextures;

    [SerializeField] private Texture2D tutorialTexture;
    [SerializeField] private Transform levelSelectPanel;
    [SerializeField] private Image levelSelectPrefab;
    [SerializeField] private Transform draggingPiece; 


    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    private int currentImageIndex = 2;

    void Start() {
        currentImageIndex = Config.startImageIndex;
        if(Config.isTutorial){
            difficulty = 2;
            StartGame(tutorialTexture);
        }
        else {
            StartGame(imageTextures[currentImageIndex]);
            currentImageIndex++;
        }
    }

    void Update() {
        if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.G))
        {
            if(currentImageIndex + 1 > imageTextures.Count)
            {
                currentImageIndex = 0;
            }
            ClearPuzzlePieces();
            StartGame(imageTextures[currentImageIndex]);
            currentImageIndex++;
        }
    }

    public void StartGame(Texture2D jigsawTexture) {
        ChangeFramePicture(jigsawTexture);
        pieces = new List<Transform>();

        dimensions = GetDimensions(jigsawTexture, difficulty);

        CreateJigsawPieces(jigsawTexture);

        Scatter();

        UpdateBorder();
    }

    private void ChangeFramePicture(Texture2D jigsawTexture){
        Renderer renderer = frame.GetComponent<Renderer>();
        if (renderer != null && renderer.materials.Length >= 2) {
            //angleText.text = "FRAME";
            Material[] materials = renderer.materials; 
            materials[1].SetTexture("_BaseMap", jigsawTexture);
            renderer.materials = materials; 
        }
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

        float halfWidth = (width * dimensions.x) / 2f;
        float halfHeight = (height * dimensions.y) / 2f;

        foreach (Transform piece in pieces) {
            Vector3 newPosition;
            bool isValidPosition;

            do {
                float x = Random.Range(panelRect.xMin, panelRect.xMax);
                float y = Random.Range(panelRect.yMin, panelRect.yMax);

                newPosition = new Vector3(x, y, 0);

                isValidPosition = (Mathf.Abs(newPosition.x) > halfWidth || Mathf.Abs(newPosition.y) > halfHeight);

            } while (!isValidPosition);

            piece.localPosition = newPosition;
            piece.localRotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

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

            logManager.AddPuzzleLog("puzzle_piece_set");

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
        foreach (Transform piece in pieces) {
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = piece.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null && grabInteractable.enabled) {
                return;
            }
        }

        Debug.Log("Puzzle komplett gelöst!");
        logManager.AddPuzzleLog("puzzle_finished");

        if (finishParticles != null) {
            finishParticles.gameObject.SetActive(true); 
            finishParticles.Play();
            StartCoroutine(HideParticleEffect()); 
        }

        if(Config.isTutorial)
        {
            EventManager.TriggerEvent(Const.Events.TutorialCompleted);
        }
        else {
            if(currentImageIndex + 1 > imageTextures.Count)
            {
                currentImageIndex = 0;
            }
            ClearPuzzlePieces();
            StartGame(imageTextures[currentImageIndex]);
            currentImageIndex++;
        }
    }

    private IEnumerator HideParticleEffect() {
        yield return new WaitForSeconds(3.5f); 

        if (finishParticles != null) {
            finishParticles.Stop(); 
            finishParticles.gameObject.SetActive(false); 
        }
    }


}