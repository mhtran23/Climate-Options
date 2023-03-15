using UnityEngine;
using Nova;

public class NovaUIBehavior : MonoBehaviour
{
    public void DisableNovaInteractableClickBehavior(GameObject gameObject)
    {
        Nova.Interactable interactable = gameObject.GetComponent<Interactable>();

        if (interactable != null)
        {
            interactable.ClickBehavior = ClickBehavior.None;
        }
    }

    public void ToggleBorderVisibility(Nova.UIBlock2D uIBlock2D)
    {
        uIBlock2D.Border.Enabled = !uIBlock2D.Border.Enabled;
    }

    public void BorderVisibility(Nova.UIBlock2D uIBlock2D, bool value)
    {
        uIBlock2D.Border.Enabled = value;
    }
}
