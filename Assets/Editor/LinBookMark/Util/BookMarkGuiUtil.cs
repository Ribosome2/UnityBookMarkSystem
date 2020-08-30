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
        
        public static void DrawRectOutline(Rect rect, Color color)
        {
            if (Event.current.type == EventType.Repaint)
            {
                Texture2D tex = EditorGUIUtility.whiteTexture;
                GUI.color = color;
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, 1f, rect.height), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 1f), tex);
                GUI.DrawTexture(new Rect(rect.xMin, rect.yMax, rect.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        public static void DrawTexture(Rect drawRect, Texture texture)
        {
            if (texture == null)
            {
                GUI.Box(drawRect,"null texture");
               
            }
            else
            {
                float textureWidth = texture.width;
                float textureHeight = texture.height;
                drawRect = CalculateDrawRect(textureWidth, textureHeight, drawRect);
                GUI.DrawTexture(drawRect, texture);
            }
        }
        
        public static void DrawSprite( Rect drawRect,Sprite sprite)
        {
            Texture2D handleTexture = sprite.texture;
            Rect uv = new Rect(sprite.rect.x / handleTexture.width, sprite.rect.y / handleTexture.height,
                sprite.rect.width / handleTexture.width, sprite.rect.height / handleTexture.height);
            GUI.backgroundColor = new Color(.6f, 1.0f, 1.0f, 0.5f);
        
            GUI.backgroundColor = Color.white;
            float spriteWidth = sprite.rect.width;
            float spriteHeight = sprite.rect.height;
            float scale = spriteWidth / spriteHeight;
            drawRect = BookMarkGuiUtil.CalculateDrawRect(spriteWidth, spriteHeight, drawRect);
            GUI.DrawTextureWithTexCoords(drawRect, handleTexture, uv);
        }

        public static Rect CalculateDrawRect(float iconWidth, float iconHeight, Rect drawRect)
        {
            float scale = iconWidth / iconHeight;
            var cellSize = drawRect.width;
            if (iconWidth < iconHeight)
            {
                drawRect.width = drawRect.height * scale;
            }
            else
            {
                drawRect.height = drawRect.width / scale;
            }
            drawRect.x += (cellSize - drawRect.width) / 2;
            drawRect.y += (cellSize - drawRect.height) / 2;
            return drawRect;
        }
    }
}