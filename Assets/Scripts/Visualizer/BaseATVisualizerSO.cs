using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Visualizer/Base Target Area")]
public class BaseATVisualizerSO : AreaTargetVisualizerSO
{
    [SerializeField] private Color m_lineColor = Color.white;
    [SerializeField] private FloatReference m_lineLifetime;

    public override void Visualize(SearchingArea searchingArea, DetectionArea detectionArea)
    {
#if UNITY_EDITOR        
        Debug.DrawLine(searchingArea.Socket.position + searchingArea.Data.Offset, 
            detectionArea.transform.position, m_lineColor, m_lineLifetime);
#endif
    }
}
