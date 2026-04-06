using UnityEngine;

//[ExecuteInEditMode]
public class Connection : MonoBehaviour
{
    public void SetRoadColor(RoadColor color)
    {
        m_roadColor = color;
    }

    public void Update()
    {
        //GameDataCollection m_data = GameDataCollection.GetInstance();
        //Node node1 = m_data.GetNodeByName(m_end1);
        //Node node2 = m_data.GetNodeByName(m_end2);
        //Vector3 pos1 = node1.GetPosition();
        //Vector3 pos2 = node2.GetPosition();

        //transform.position = (pos1 + pos2) / 2;
        //transform.localScale = new Vector3(Vector3.Magnitude(pos2 - pos1), 0.3f, 0.3f);
        //transform.rotation = Quaternion.FromToRotation(Vector3.right, pos2 - pos1);

    }

    [SerializeField]
    public StationName m_end1, m_end2;

    [SerializeField]
    public int m_totalCost;

    [SerializeField]
    public int m_tunnelCost;

    [SerializeField]
    public int m_boatCost;

    [SerializeField]
    public RoadColor m_roadColor;
}
