using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        
        public void SetMessage(string message)
        {
            messageText.text = message;
        }
        
        public void SetColor(Color color)
        {
            messageText.color = color;
        }
    }
}