using UnityEngine;

public class ColorPicker : MonoBehaviour 
{
    private Camera m_camera;
    public BoxCollider pickerCollider;

    private static Texture2D m_staticRectTexture;
    private static GUIStyle m_staticRectStyle;
    
    private bool m_grab;
    private Texture2D m_screenRenderTexture;
    private static Vector3 m_pixelPosition = Vector3.zero;
    private Color m_pickedColor = Color.white;

    // 新增变量：窗口控制
    private Rect m_windowRect = new Rect(0, 0, 150, 200);
    private bool m_isDragging = false;
    private Vector2 m_dragStart;

    void Awake() 
    {
        m_camera = GetComponent<Camera>();
        if (m_camera == null) 
        {
            Debug.LogError("You need to attach this script to a camera gameObject!");
            return;
        }

        if (pickerCollider == null) 
        {
            pickerCollider = gameObject.AddComponent<BoxCollider>();
            pickerCollider.center = Vector3.zero;
            pickerCollider.center += m_camera.transform.worldToLocalMatrix.MultiplyVector(m_camera.transform.forward) * (m_camera.nearClipPlane + 0.2f);
            pickerCollider.size = new Vector3(Screen.width, Screen.height, 0.1f);
        }
    }
    
    void OnMouseDown() 
    {
        m_grab = true;
        m_pixelPosition = Input.mousePosition;
    }

    void OnPostRender() 
    {
        if (m_grab) 
        {
            m_screenRenderTexture = new Texture2D(Screen.width, Screen.height);
            m_screenRenderTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            m_screenRenderTexture.Apply();
            m_pickedColor = m_screenRenderTexture.GetPixel(Mathf.FloorToInt(m_pixelPosition.x), Mathf.FloorToInt(m_pixelPosition.y));
            m_grab = false;
        }
    }
    
    void OnGUI() 
    {
        // 绘制可拖动窗口
        m_windowRect = GUI.Window(0, m_windowRect, ColorPickerWindow, "Color Picker");
    }

    void ColorPickerWindow(int windowID)
    {
        // 自定义拖动区域
        Rect dragRect = new Rect(0, 0, m_windowRect.width, m_windowRect.height);
        GUI.DragWindow(dragRect);

        // 绘制颜色方块
        GUIDrawRect(new Rect(20, 30, 80, 80), m_pickedColor);

        // 绘制颜色信息
        GUI.Label(new Rect(10, 120, 120, 20), "R: " + System.Math.Round((double)m_pickedColor.r, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.r * 255)+ ")");
        GUI.Label(new Rect(10, 140, 120, 20), "G: " + System.Math.Round((double)m_pickedColor.g, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.g * 255)+ ")");
        GUI.Label(new Rect(10, 160, 120, 20), "B: " + System.Math.Round((double)m_pickedColor.b, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.b * 255)+ ")");
        GUI.Label(new Rect(10, 180, 120, 20), "A: " + System.Math.Round((double)m_pickedColor.a, 4) + "\t(" + Mathf.FloorToInt(m_pickedColor.a * 255)+ ")");
    }
    
    public static void GUIDrawRect(Rect position, Color color)
    {
        if(m_staticRectTexture == null)
        {
            m_staticRectTexture = new Texture2D(1, 1);
        }
        
        if(m_staticRectStyle == null)
        {
            m_staticRectStyle = new GUIStyle();
        }
        
        m_staticRectTexture.SetPixel(0, 0, color);
        m_staticRectTexture.Apply();
        
        m_staticRectStyle.normal.background = m_staticRectTexture;
        
        GUI.Box(position, GUIContent.none, m_staticRectStyle);
    }
    
}