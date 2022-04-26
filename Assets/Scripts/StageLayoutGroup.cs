namespace BugGame.Canvas
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>Which corner is the starting corner for the grid.</summary>
    public enum Corner
    {
        /// <summary>Upper Left corner.</summary>
        UpperLeft = 0,
        /// <summary>Upper Right corner.</summary>
        UpperRight = 1,
        /// <summary>Lower Left corner.</summary>
        LowerLeft = 2,
        /// <summary>Lower Right corner.</summary>
        LowerRight = 3
    }

    /// <summary>The grid axis we are looking at.</summary>
    /// <remarks>As the storage is a [][] we make access easier by passing a axis.</remarks>
    public enum Axis
    {
        /// <summary>Horizontal axis</summary>
        Horizontal = 0,
        /// <summary>Vertical axis.</summary>
        Vertical = 1
    }

    /// <summary>Constraint type on either the number of columns or rows.</summary>
    public enum Constraint
    {
        /// <summary>Don't constrain the number of rows or columns.</summary>
        Flexible = 0,
        /// <summary>Constrain the number of columns to a specified number.</summary>
        FixedColumnCount = 1,
        /// <summary>Constraint the number of rows to a specified number.</summary>
        FixedRowCount = 2
    }


    public partial class StageLayoutGroup : LayoutGroup
    {
        /// <summary>The size to use for each cell in the grid.</summary>
        public Vector2 CellSize {
            get => m_CellSize;
            set => SetProperty(ref m_CellSize, value);
        }
        [SerializeField] protected Vector2 m_CellSize = new Vector2(100, 100);

        /// <summary>
        /// The spacing to use between layout elements in the grid on both axises.
        /// </summary>
        public Vector2 Spacing {
            get => m_Spacing;
            set => SetProperty(ref m_Spacing, value);
        }
        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        public Corner StartCorner;

        public Axis StartAxis;
        public Constraint Constraint;
        public int m_ConstraintCount;

        protected StageLayoutGroup()
        { }

        public override void SetLayoutHorizontal()
        {
            // Only set the sizes when invoked for horizontal axis, not the positions.
            var rectChildrenCount = rectChildren.Count;
            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform rect = rectChildren[i];

                m_Tracker.Add(this, rect,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.SizeDelta);

                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = CellSize;
            }
        }

        public override void SetLayoutVertical()
        {
            var rectChildrenCount = rectChildren.Count;
            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            int cellCountX = 1;
            int cellCountY = 1;
            if (Constraint == Constraint.FixedColumnCount)
            {
                cellCountX = m_ConstraintCount;

                if (rectChildrenCount > cellCountX)
                    cellCountY = rectChildrenCount / cellCountX + (rectChildrenCount % cellCountX > 0 ? 1 : 0);
            }
            else if (Constraint == Constraint.FixedRowCount)
            {
                cellCountY = m_ConstraintCount;

                if (rectChildrenCount > cellCountY)
                    cellCountX = rectChildrenCount / cellCountY + (rectChildrenCount % cellCountY > 0 ? 1 : 0);
            }
            else
            {
                if (CellSize.x + Spacing.x <= 0)
                    cellCountX = int.MaxValue;
                else
                    cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));

                if (CellSize.y + Spacing.y <= 0)
                    cellCountY = int.MaxValue;
                else
                    cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y)));
            }

            int cornerX = (int)StartCorner % 2;
            int cornerY = (int)StartCorner / 2;

            int cellsPerMainAxis, actualCellCountX, actualCellCountY;
            if (StartAxis == Axis.Horizontal)
            {
                cellsPerMainAxis = cellCountX;
                actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildrenCount);
                actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
            }
            else
            {
                cellsPerMainAxis = cellCountY;
                actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildrenCount);
                actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildrenCount / (float)cellsPerMainAxis));
            }

            Vector2 requiredSpace = new Vector2(
                actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spacing.x,
                actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spacing.y
            );
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
            );

            for (int i = 0; i < rectChildrenCount; i++)
            {
                int positionX;
                int positionY;
                if (StartAxis == Axis.Horizontal)
                {
                    positionX = i % cellsPerMainAxis;
                    positionY = i / cellsPerMainAxis;
                }
                else
                {
                    positionX = i / cellsPerMainAxis;
                    positionY = i % cellsPerMainAxis;
                }

                if (cornerX == 1)
                    positionX = actualCellCountX - 1 - positionX;
                if (cornerY == 1)
                    positionY = actualCellCountY - 1 - positionY;

                SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (CellSize[0] + Spacing[0]) * positionX, CellSize[0]);
                SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (CellSize[1] + Spacing[1]) * positionY, CellSize[1]);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
            throw new System.NotImplementedException();
        }
    }
}
