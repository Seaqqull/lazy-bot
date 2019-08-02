using LazyBot.Area.Searching;
using UnityEngine;

[CreateAssetMenu(menuName = "Visualizer/Base Circle Area")]
public class BaseSAVisualizerSO : SearchingAreaVisualizerSO
{
    [SerializeField] private Color m_lineColor = Color.white;


    public override void Visuzlize(SearchingArea searchingArea)
    {
        Vector3 positionWithOffset = searchingArea.Socket.position + searchingArea.Data.Offset;
        UnityEditor.Handles.color = m_lineColor;

        UnityEditor.Handles.DrawWireArc(positionWithOffset,
            Vector3.up, Vector3.forward, 360.0f, searchingArea.Data.Radius);

        if (searchingArea.Data.Angle != 360.0f)
        {
            UnityEditor.Handles.DrawLine(positionWithOffset,
                positionWithOffset + LazyBot.Utility.Data.FloatHelper.DirFromAngleY(
                    -searchingArea.Data.Angle / 2, searchingArea.transform.eulerAngles.y,
                    searchingArea.Data.Rotation.y, false)
                * searchingArea.Data.Radius);

            UnityEditor.Handles.DrawLine(positionWithOffset,
                positionWithOffset + LazyBot.Utility.Data.FloatHelper.DirFromAngleY(
                    searchingArea.Data.Angle / 2, searchingArea.transform.eulerAngles.y,
                    searchingArea.Data.Rotation.y, false) * searchingArea.Data.Radius);
        }
    }    
}
