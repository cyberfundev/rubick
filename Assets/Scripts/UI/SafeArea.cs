using UnityEngine;

namespace UI
{
    public class SafeArea : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Canvas _canvas;
        private void Start()
        {
            Vector2 safeAreaPos = Screen.safeArea.position;
            
            var anchorMin = safeAreaPos;
            var anchorMax = safeAreaPos + Screen.safeArea.size;
            
            var pixelRect = _canvas.pixelRect;
            anchorMin.x /= pixelRect.width;
            anchorMin.y /= pixelRect.height;
            anchorMax.x /= pixelRect.width;
            anchorMax.y /= pixelRect.height;

            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;
        }
    }
}
