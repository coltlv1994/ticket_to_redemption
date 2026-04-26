using UnityEngine;
using UnityEngine.UI;

public class ClickableCard : MonoBehaviour
{
    [SerializeField] public int cardIndex;

    public void ToggleSelection()
    {
        bool isSelected = GameDataCollection.GetInstance().ToggleCardSelection(cardIndex);

        if (isSelected)
        {
            GetComponent<RawImage>().color = Color.yellow; // Highlight selected card

        }
        else
        {
            GetComponent<RawImage>().color = Color.white; // Reset color for deselected card
        }
    }
}
