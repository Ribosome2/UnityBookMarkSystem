using UnityEditor;
using UnityEngine;

namespace LinBookMark
{
    public static class BookMarkGuiUtil
    {
        private static int MouseDeltaReaderHash = "MouseDeltaReader".GetHashCode();
        public static Vector2 MouseDeltaReaderLastPos;
        public static Vector2 MouseDeltaReader(Rect position, bool activated)
        {
            int controlId = GUIUtility.GetControlID(MouseDeltaReaderHash, FocusType.Passive, position);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    if (activated && GUIUtility.hotControl == 0 && (position.Contains(current.mousePosition) && current.button == 0))
                    {
                        GUIUtility.hotControl = controlId;
                        GUIUtility.keyboardControl = 0;
                        MouseDeltaReaderLastPos = current.mousePosition;
                        current.Use();
                        break;
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId && current.button == 0)
                    {
                        GUIUtility.hotControl = 0;
                        current.Use();
                        break;
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId)
                    {
                        Vector2 vector2_1 = current.mousePosition;
                        Vector2 vector2_2 = vector2_1 - MouseDeltaReaderLastPos;
                        MouseDeltaReaderLastPos = vector2_1;
                        current.Use();
                        return vector2_2;
                    }
                    break;
            }
            return Vector2.zero;
        }
        
        public static Rect HandleHorizontalSplitter(
            Rect dragRect,
            float width,
            float minLeftSide,
            float minRightSide)
        {
            if (Event.current.type == UnityEngine.EventType.Repaint)
                EditorGUIUtility.AddCursorRect(dragRect, MouseCursor.SplitResizeLeftRight);
            float num = 0.0f;
            float x =MouseDeltaReader(dragRect, true).x;
            if ((double) x != 0.0)
            {
                dragRect.x += x;
                num = Mathf.Clamp(dragRect.x, minLeftSide, width - minRightSide);
            }
            if ((double) dragRect.x > (double) width - (double) minRightSide)
                num = width - minRightSide;
            if ((double) num > 0.0)
                dragRect.x = num;
            return dragRect;
        }
        
        public static void DrawHorizontalSplitter(Rect dragRect)
        {
            if (Event.current.type != EventType.Repaint)
                return;
            Color color = GUI.color;
            GUI.color *= !EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.12f, 0.12f, 0.12f, 1.333f);
            GUI.DrawTexture(new Rect(dragRect.x - 1f, dragRect.y, 1f, dragRect.height), (Texture) EditorGUIUtility.whiteTexture);
            GUI.color = color;
        }
    }
}