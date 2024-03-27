using UnityEngine;
using UnityEngine.UI;

namespace UI.Layout_Groups
{
    [AddComponentMenu("Layout/Extensions/Fanned Layout")]
    public class FannedLayout : LayoutGroup
    {
        public float degreesPerChild = 5f;
        private float _arcDegrees = 180f; // The total arc span of the hand
        public float arcRadius = 300f; // The radius of the arc

#if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();
            CalculateLayout();
        }
#endif
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            CalculateLayout();
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
            SetCardsLayout();
        }

        public override void SetLayoutVertical()
        {
            SetCardsLayout();
        }

        private void CalculateLayout()
        {
            // Calculate your layout here, setting up the required data structures
        }

        private void SetCardsLayout()
        {
            int childCount = rectChildren.Count;
            
            if (childCount == 0)
            {
                return;
            }
            
            _arcDegrees = rectChildren.Count > 0 ? degreesPerChild * childCount : 50f;
            
            // Handle the case with only one card
            if (childCount == 1)
            {
                rectChildren[0].localPosition = new Vector3(0, 0, 0); // or any desired position
                rectChildren[0].localRotation = Quaternion.identity; // no rotation needed for a single card
                return;
            }

            float angleStep = _arcDegrees / (rectChildren.Count - 1);
            Vector3 centerPoint = new Vector3(0, -arcRadius, 0);

            for (int i = 0; i < rectChildren.Count; i++)
            {
                float cardAngle = (angleStep * i) - (_arcDegrees / 2);
                Quaternion rotation = Quaternion.Euler(0, 0, cardAngle);
                Vector3 position = rotation * new Vector3(0, arcRadius, 0) + centerPoint;

                rectChildren[i].localPosition = position;
                rectChildren[i].localRotation = rotation;
            }
        }

        // Ensure we re-calculate the layout when the component is enabled
        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateLayout();
            SetCardsLayout();
        }
        
    }
}