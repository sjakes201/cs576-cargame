using UnityEngine;

public class StopLight : MonoBehaviour
{
    public enum LightColor { Green, Yellow, Red }
    private LightColor currentLightColor;

    private Renderer lightRenderer;
    private float timer;

    void Start()
    {
        lightRenderer = GetComponent<Renderer>();
        SetLightColor(LightColor.Green);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (currentLightColor == LightColor.Green && timer >= 6f)
        {
            SetLightColor(LightColor.Yellow);
        }
        else if (currentLightColor == LightColor.Yellow && timer >= 2f)
        {
            SetLightColor(LightColor.Red);
        }
        else if (currentLightColor == LightColor.Red && timer >= 8f)
        {
            SetLightColor(LightColor.Green);
        }
    }

    public void SetLightColor(LightColor color)
    {
        currentLightColor = color;
        timer = 0f;

        switch (currentLightColor)
        {
            case LightColor.Green:
                lightRenderer.material.color = Color.green;
                break;
            case LightColor.Yellow:
                lightRenderer.material.color = Color.yellow;
                break;
            case LightColor.Red:
                lightRenderer.material.color = Color.red;
                break;
        }
    }

    public LightColor GetCurrentLightColor()
    {
        return currentLightColor;
    }
}
