using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RendererSortingOrder : MonoBehaviour
{
    public string sortingLayerName = "Default";
    public int sortingOrder = 0;

    private Renderer rendererComponent;

    void Start()
    {
        SetOrder();
    }

    [ContextMenu("Set Order")]
    public void SetOrder()
    {
        rendererComponent = GetComponent<Renderer>();

        // Set the sorting layer and order for any Renderer component
        if (rendererComponent != null)
        {
            rendererComponent.sortingLayerName = sortingLayerName;
            rendererComponent.sortingOrder = sortingOrder;
        }
    }

    private void OnValidate()
    {
        SetOrder();
    }
}
