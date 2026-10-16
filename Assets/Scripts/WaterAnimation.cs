using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    [SerializeField] private Renderer waterRenderer;
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.03f, 0.02f);

    private Material waterMaterial;
    private Vector2 offset;

    private void Awake()
    {
        waterMaterial = waterRenderer.material;
    }

    private void Update()
    {
        offset += scrollSpeed * Time.deltaTime;
        waterMaterial.mainTextureOffset = offset;
    }
}