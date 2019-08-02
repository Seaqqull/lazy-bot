using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LazyBot.Visualizer
{
    /// <summary>
    /// Visualize audibility or loudness of audio.
    /// </summary>
    public class AudioSourceVisualizer : MonoBehaviour
    {
        [SerializeField] private LazyBot.Audio.AudioContainer m_audios;

        /// <summary>
        /// Is transition visualized or drawn as two siple circle.
        /// </summary>
        [SerializeField] private bool m_vizualize = true;
        /// <summary>
        /// Is drawn detection or 3D.
        /// </summary>
        [SerializeField] private bool m_drawDetection = true;
        /// <summary>
        /// Is drawn only when selected.
        /// </summary>
        [SerializeField] private bool m_drawOnSelected = true;

        /// <summary>
        /// Index of drawn audio.
        /// </summary>
        [SerializeField] private int m_drawZone = 0;
        /// <summary>
        /// Count of steps, on visualizations.
        /// </summary>
        [SerializeField] [Range(0.1f, sbyte.MaxValue)] private int m_step = 10;
        [SerializeField] [Range(0.0f, 1.0f)] private float m_alpha = 0.05f;

        [SerializeField] private Color m_colorZone = Color.black;
        [SerializeField] private Color m_colorNoiseBad = Color.red;
        [SerializeField] private Color m_colorNoiseGood = Color.green;


        private void OnDrawGizmos()
        {
            if ((!m_audios) || 
                (m_drawOnSelected) ||
                (m_audios.Audios.Count == 0) ||
                ((m_drawZone < 0) || (m_drawZone >= m_audios.Audios.Count))) return;

            if (m_drawDetection)
                DrawZone(m_audios.Audios[m_drawZone].InnerRadiusDetection, m_audios.Audios[m_drawZone].OutherRadiusDetection);
            else
                DrawZone(m_audios.Audios[m_drawZone].InnerRadius3D, m_audios.Audios[m_drawZone].OutherRadius3D);
        }

        private void OnDrawGizmosSelected()
        {
            if ((!m_audios) ||
                (!m_drawOnSelected) ||
                (m_audios.Audios.Count == 0) ||
                ((m_drawZone < 0) || (m_drawZone >= m_audios.Audios.Count))) return;

            if (m_drawDetection)
                DrawZone(m_audios.Audios[m_drawZone].InnerRadiusDetection, m_audios.Audios[m_drawZone].OutherRadiusDetection);
            else
                DrawZone(m_audios.Audios[m_drawZone].InnerRadius3D, m_audios.Audios[m_drawZone].OutherRadius3D);
        }


        /// <summary>
        /// Draws circle, that represents audibility or loudness of audio.
        /// </summary>
        /// <param name="innerRadius">Minimal raius.</param>
        /// <param name="outherRadius">Maximal radius.</param>
        private void DrawZone(float innerRadius, float outherRadius)
        {
#if UNITY_EDITOR
            if (m_vizualize)
            {
                float progress;
                float stepLength = (outherRadius - innerRadius) / m_step;

                for (float i = outherRadius; i >= innerRadius; i -= stepLength)
                {
                    progress = 1 - LazyBot.Utility.Data.FloatHelper.Map(i, innerRadius, outherRadius, 0, 1);

                    UnityEditor.Handles.color = new Color(
                        Mathf.Lerp(m_colorNoiseBad.r, m_colorNoiseGood.r, progress),
                        Mathf.Lerp(m_colorNoiseBad.g, m_colorNoiseGood.g, progress),
                        Mathf.Lerp(m_colorNoiseBad.b, m_colorNoiseGood.b, progress), m_alpha);

                    UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, -transform.right, 360, i);
                }
            }
            else
            {
                UnityEditor.Handles.color = m_colorZone;

                UnityEditor.Handles.DrawWireArc(transform.position,
                    Vector3.up, Vector3.forward, 360, innerRadius);
                UnityEditor.Handles.DrawWireArc(transform.position,
                    Vector3.up, Vector3.forward, 360, outherRadius);
            }
#endif
        }

    }
}
