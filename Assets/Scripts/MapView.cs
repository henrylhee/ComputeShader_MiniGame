using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    RenderTexture renderTexture;

    [SerializeField] Color mouseColor;
    [SerializeField] Color emptyColor;
    [SerializeField, Range(0f,0.2f)] float mouseRadius = 0.05f;

    RawImage image;
    Texture2D dataTexture;
    Texture2D colorTexture;
    Color[] texColor;

    Resolution resolution;
    float uvX;
    float uvY;
    Vector2 mousePos;


    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        resolution = GameModel.Instance.resolution;

        dataTexture = new Texture2D(resolution.width, resolution.height);
        colorTexture = new Texture2D(resolution.width, resolution.height);

        image = GetComponent<RawImage>();

        GameModel.Instance.OnStatesUpdated.AddListener(UpdateView);

        colorTexture.Apply();
        //image.texture = colorTexture;

        //InitializeShader();
        Debug.Log("Mapview initialized.");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture != null)
        {
            Graphics.Blit(renderTexture, destination);         
        }
    }

    private void UpdateView()
    {
        //colorTexture.SetPixels(GameModel.Instance.map.colorMap, 0);
        //colorTexture.Apply();
        //UpdateShader();
        renderTexture = GameModel.Instance.map.renderTexture;
    }

    private void InitializeShader()
    {
        image.material.SetColor("_MouseColor", mouseColor);
        image.material.SetColor("_EMPTYColor", emptyColor);
        image.material.SetFloat("_MouseRadius", mouseRadius);
        image.material.SetTexture("_ColorTexture", colorTexture);
        image.material.SetFloat("_Width", resolution.width);
    }

    private void UpdateShader()
    {
        image.material.SetTexture("_ColorTexture", colorTexture);
    }


    public void UpdateInput(float x, float y)
    {
        uvX = x;
        uvY = y;
    }

    private Color[] Fill(int size,  Color color)
    {
        Color[] result = new Color[size];
        for (int i = 0; i < size; i++)
        {
            result[i] = color;
        }
        return result;
    }

    private int PositionToInt(Vector2 position, int width)
    {
        return ((int)position.y-1) * width + width;
    }

    private List<int> FindClosePositions(Vector2 position, int start, int width, int height, float radius)
    {
        List<int> result = new List<int>();
        result.Add(start);

        Dictionary<int,int> limits = new Dictionary<int, int>();




        return result;
    }
}
