using TMPro;
using UnityEngine;

public class ShowFPS : MonoBehaviour
{
    private TextMeshProUGUI fpsText;
    private float deltaTime;


    private void Start()
    {
        fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();
    }
}