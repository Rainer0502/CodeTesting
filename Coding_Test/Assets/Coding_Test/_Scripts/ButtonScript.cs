using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class ButtonScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerUpHandler
{
    #region Vars
    [SerializeField] private GameObject tooltip = null;
    [SerializeField] private TextMeshProUGUI tooltipText = null;
    [SerializeField] private string tooltips = "";

    [SerializeField] private GameObject popup = null;
    [SerializeField] private TextMeshProUGUI popupText = null;
    [SerializeField] private string popups = "";

    [SerializeField] private Color[] colors = { Color.red, Color.green, Color.blue };

    private bool isDragging = false;
    private int currentIndex = -1;
    #endregion

    #region UnityFunctions
    void Start()
    {
        tooltip.SetActive(false);
        popup.SetActive(false);
        currentIndex = -1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Invoke("ShowTooltip", 0.5f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke();
        tooltip.SetActive(false);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;

        popup.SetActive(false);
        tooltip.SetActive(false);
        isDragging = true;

        CheckOverlap();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            popup.SetActive(true);
            popupText.text = popups;
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            currentIndex = (currentIndex + 1) % 3;
            GetComponent<Image>().color = colors[currentIndex];
            tooltipText.text = tooltips;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            CheckOverlap();
        }
    }
    #endregion

    #region Functions
    void CheckOverlap()
    {
        foreach (GameObject button in GameObject.FindGameObjectsWithTag("ButtonSC"))
        {
            if (button != gameObject && RectTransformUtility.RectangleContainsScreenPoint(button.GetComponent<RectTransform>(), Input.mousePosition))
            {
                Vector2 direction = (Vector2)button.transform.position - (Vector2)transform.position;
                button.transform.position = (Vector2)button.transform.position + direction.normalized * 10f;

                // Check if the button is outside the screen bounds
                float halfWidth = button.GetComponent<RectTransform>().rect.width / 2f;
                float halfHeight = button.GetComponent<RectTransform>().rect.height / 2f;
                float xMin = halfWidth;
                float xMax = Screen.width - halfWidth;
                float yMin = halfHeight;
                float yMax = Screen.height - halfHeight;
                Vector2 position = button.transform.position;
                position.x = Mathf.Clamp(position.x, xMin, xMax);
                position.y = Mathf.Clamp(position.y, yMin, yMax);
                button.transform.position = position;
            }
        }
    }
    void ShowTooltip()
    {
        tooltip.SetActive(true);
        tooltipText.text = tooltips;
    }
    #endregion
}