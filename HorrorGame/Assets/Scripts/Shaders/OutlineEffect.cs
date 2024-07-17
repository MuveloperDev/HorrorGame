using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    public Material outlineMaterial;

    private Renderer objectRenderer;
    private Material[] originalMaterials;

    async void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        originalMaterials = objectRenderer.materials;
        var material = await ResourcesManager.Instance.LoadAssetAsyncGeneric<Material>("Materials/OutlineMaterial");
        outlineMaterial = material;
    }

    public void RemoveOutline()
    {
        objectRenderer.materials = originalMaterials;
    }
    public void ApplyOutline()
    {
        // Create new materials array with additional slot for outline
        Material[] newMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = originalMaterials[i];
        }

        // Assign the outline material to the last slot
        newMaterials[newMaterials.Length - 1] = outlineMaterial;

        // Apply the new materials array to the renderer
        objectRenderer.materials = newMaterials;
    }
}