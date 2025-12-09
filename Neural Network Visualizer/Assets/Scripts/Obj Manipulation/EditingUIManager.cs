using UnityEngine;
using TMPro;

public class EditingUIManager : MonoBehaviour
{
    public static EditingUIManager current;

    [Header("UI References")]
    public GameObject editingUIPanel;
    public TMP_Text objectNameText;
    public TMP_Text storedNumberText;
    public TMP_InputField editNumberInputField;

    private GameObject currentEditingObject;
    private Layer currentLayer;

    private void Awake()
    {
        current = this;

        // Hide UI panel at start
        if (editingUIPanel != null)
        {
            editingUIPanel.SetActive(false);
        }
    }

    public void ShowEditingUI(GameObject targetObject)
    {
        // Get the Layer component from the target object
        Layer layer = targetObject.GetComponent<Layer>();

        if (layer == null)
        {
            Debug.LogWarning($"Object {targetObject.name} does not have a Layer component!");
            return;
        }

        currentEditingObject = targetObject;
        currentLayer = layer;

        // Update UI elements
        if (objectNameText != null)
        {
            objectNameText.text = $"Editing: {targetObject.name}";
        }

        if (storedNumberText != null)
        {
            storedNumberText.text = $"Current Value: {layer.GetStoredNumber()}";
        }

        if (editNumberInputField != null)
        {
            editNumberInputField.text = layer.GetStoredNumber().ToString();
        }

        // Show the panel
        if (editingUIPanel != null)
        {
            editingUIPanel.SetActive(true);
        }

        Debug.Log($"Opened editing UI for {targetObject.name} with stored number: {layer.GetStoredNumber()}");
    }

    public void HideEditingUI()
    {
        if (editingUIPanel != null)
        {
            editingUIPanel.SetActive(false);
        }

        currentEditingObject = null;
        currentLayer = null;
    }

    // Call this method from a "Save" button in the UI
    public void SaveChanges()
    {
        if (currentLayer != null && editNumberInputField != null)
        {
            if (double.TryParse(editNumberInputField.text, out double newValue))
            {
                // Update the layer's stored number
                currentLayer.SetStoredNumber(newValue);
                Debug.Log($"Saved new value: {newValue} to {currentEditingObject.name}");

                // Update the display
                if (storedNumberText != null)
                {
                    storedNumberText.text = $"Current Value: {newValue}";
                }
            }
            else
            {
                Debug.LogWarning("Invalid number format!");
            }
        }
    }

    // Call this method from a "Close" button in the UI
    public void CloseEditingUI()
    {
        HideEditingUI();
    }
}