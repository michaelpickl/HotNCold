using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
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


    private List<Transform> pieces;
    private Vector2Int dimensions;
    private float width;
    private float height;

    void Start() {
        foreach (Texture2D texture in imageTextures) {
            StartGame(texture);
        }
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
                    -1);
                piece.localScale = new Vector3(width, height, 1f);

                piece.name = $"Piece {(row * dimensions.x) + col}";
                pieces.Add(piece);
                
                float width1 = 1f / dimensions.x;
                float height1 = 1f / dimensions.y;

                Vector2[] uv = new Vector2[4];
                uv[0] = new Vector2(width1 * col, height1 * row);
                uv[1] = new Vector2(width1 * (col + 1), height1 * row);
                uv[2] = new Vector2(width1 * col, height1 * (row + 1));
                uv[3] = new Vector2(width1 * (col + 1), height1 * (row + 1));

                Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
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

    
}