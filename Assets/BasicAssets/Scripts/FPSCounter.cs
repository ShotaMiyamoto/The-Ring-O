using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float m_updateInterval = 0.5f;

    private float m_accum = default;
    private int m_frames = default;
    private float m_timeleft = default;
    private float m_fps = default;
    private GUIStyle style = default;

    [SerializeField] private bool changePosition = false;
    [SerializeField] private Rect pos = new Rect(0, 0, 0, 0);
    [SerializeField] private float x = default;
    [SerializeField] private float y = default;
    [SerializeField] private float width = default;
    [SerializeField] private float height = default;

    bool changeRoutineFlag = true;

    private void Start()
    {
        pos = new Rect(x, y, width, height);
        style = new GUIStyle();
        style.fontSize = 60;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;

        if (0 < m_timeleft) return;

        m_fps = m_accum / m_frames;
        m_timeleft = m_updateInterval;
        m_accum = 0;
        m_frames = 0;
    }

    private void OnGUI()
    {
        if (changeRoutineFlag)
        {
            if (!(Event.current.type == EventType.Layout))
            {
                return;
            }

            changeRoutineFlag = false;
        }

        if (!changePosition)
        {
            GUILayout.Label("FPS: " + m_fps.ToString("f2"), style);
        }
        else
        {
            GUI.Label(pos, "FPS: " + m_fps.ToString("f2"), style);
        }
        
    }
}
