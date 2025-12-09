using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EditingUIManager : MonoBehaviour
{
    public static EditingUIManager current;

    [Header("UI References")]
    public GameObject editingUIPanel;
    public TMP_Text objectNameText;
    public TMP_Text storedNumberText;
    public Slider editNumberSlider;
    public TMP_Text sliderValueText;

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

        // Add slider listener
        if (editNumberSlider != null)
        {
            // Set slider to whole numbers only
            editNumberSlider.wholeNumbers = true;
            editNumberSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        // Round to nearest whole number (extra safety)
        int roundedValue = Mathf.RoundToInt(value);
        
        // Update the slider value text in real-time
        if (sliderValueText != null)
        {
            sliderValueText.text = roundedValue.ToString();
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
            objectNameText.text = $"{targetObject.name}";
        }

        double storedValue = layer.GetStoredNumber();
        int roundedValue = Mathf.RoundToInt((float)storedValue);

        if (storedNumberText != null)
        {
            storedNumberText.text = $"{roundedValue}";
        }

        if (editNumberSlider != null)
        {
            editNumberSlider.value = roundedValue;
        }

        if (sliderValueText != null)
        {
            sliderValueText.text = roundedValue.ToString();
        }

        // Show the panel
        if (editingUIPanel != null)
        {
            editingUIPanel.SetActive(true);
        }

        Debug.Log($"Opened editing UI for {targetObject.name} with stored number: {roundedValue}");
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
        if (currentLayer != null && editNumberSlider != null)
        {
            int roundedValue = Mathf.RoundToInt(editNumberSlider.value);
            
            // Update the layer's stored number
            currentLayer.SetStoredNumber(roundedValue);
            Debug.Log($"Saved new value: {roundedValue} to {currentEditingObject.name}");
            
            // Update the display
            if (storedNumberText != null)
            {
                storedNumberText.text = $"{roundedValue}";
            }
        }
    }

    // Call this method from a "Close" button in the UI
    public void CloseEditingUI()
    {
        HideEditingUI();
    }

    private void OnDestroy()
    {
        // Clean up slider listener
        if (editNumberSlider != null)
        {
            editNumberSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}