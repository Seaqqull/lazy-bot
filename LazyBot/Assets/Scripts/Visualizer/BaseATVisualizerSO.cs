using LazyBot.Area.Detection;
using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Visualizer/Base Target Area")]
public class BaseATVisualizerSO : AreaTargetVisualizerSO
{
    [SerializeField] private Color _lineColor = Color.white;

#pragma warning disable 0649
    [SerializeField] private FloatReference _lineLifetime;
#pragma warning restore 0649

    public override void Visualize(SearchingArea searchingArea, DetectionArea detectionArea)
    {
#if UNITY_EDITOR        
        Debug.DrawLine(searchingArea.Socket.position + searchingArea.Data.Offset, 
            detectionArea.transform.position, _lineColor, _lineLifetime);
#endif
    }
}
