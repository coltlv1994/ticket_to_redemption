using UnityEngine;
using MapData;
using TMPro;

public class Station : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_data = GameDataCollection.GetInstance();
        m_node = m_data.GetNodeByName(m_name);
        m_textMesh.text = m_name.ToString();
        enabled = false; // station don't update
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnClick()
    {
        print("clicked station " + m_name);
    }

    [SerializeField]
    public StationName m_name;
    [SerializeField]
    private TextMeshPro m_textMesh;

    private Node m_node; // internal node
    private GameDataCollection m_data;
}
