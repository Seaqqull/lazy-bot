using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Visualizer/Base Target Area")]
public class BaseATVisualizerSO : AreaTargetVisualizerSO
{
    [SerializeField] private Color _lineColor = Color.white;
    [SerializeField] private FloatReference _lineLifetime;

    public override void Visualize(SearchingArea searchingArea, DetectionArea detectionArea)
    {
#if UNITY_EDITOR        
        Debug.DrawLine(searchingArea.Socket.position + searchingArea.Data.Offset, 
            detectionArea.transform.position, _lineColor, _lineLifetime);
#endif
    }
}
