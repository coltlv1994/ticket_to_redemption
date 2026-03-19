using MapData;
using System.Collections.Generic;
using UnityEngine;
// Route is for connecting neighbors
// first two is start and end
// last three is total cost, tunnel and boat
using Route = System.Tuple<MapData.StationName, MapData.StationName, int, int, int>;

public class GameDataCollection : MonoBehaviour
{
    // make a singleton for game data collection

    public static GameDataCollection GetInstance()
    {
        return m_instance;
    }

    public void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;

            // This class will NOT call Update()
            enabled = false;

            routes = new List<Route>();
            mapData = new Dictionary<StationName, Node>();

            GenerateMapRoute();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateMapRoute()
    {
        routes.Add(new Route(StationName.COLTER, StationName.ISABELLA, 2, 0, 0));
        routes.Add(new Route(StationName.COLTER, StationName.DAKOTA, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.DAKOTA, 3, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.WALLACE, 4, 0, 0));
        routes.Add(new Route(StationName.ISABELLA, StationName.PRONGHORN, 4, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.DAKOTA, 5, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BUCCHUS, 1, 0, 0));
        routes.Add(new Route(StationName.WAPITI, StationName.BRANDWINE, 6, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.DAKOTA, 4, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OIL_FIELD, 3, 0, 0));
        routes.Add(new Route(StationName.BUCCHUS, StationName.OCREAGB, 3, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.OCREAGB, 3, 0, 0));
        routes.Add(new Route(StationName.BRANDWINE, StationName.ANNESBURG, 3, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.OIL_FIELD, 5, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.ANNESBURG, 4, 0, 0));
        routes.Add(new Route(StationName.OCREAGB, StationName.BUTCHER, 3, 0, 0));
        routes.Add(new Route(StationName.ANNESBURG, StationName.VAN_HORN, 2, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.WALLACE, 3, 0, 0));
        routes.Add(new Route(StationName.PRONGHORN, StationName.OWANJILA, 3, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.DAKOTA, 3, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.VALENTINE, 3, 0, 0));
        routes.Add(new Route(StationName.WALLACE, StationName.RIGGS, 3, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.DAKOTA, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.OIL_FIELD, 2, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.HEARTLAND, 3, 0, 0));
        routes.Add(new Route(StationName.VALENTINE, StationName.FLATNECK, 3, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.EMERALD, 3, 0, 0));
        routes.Add(new Route(StationName.OIL_FIELD, StationName.HEARTLAND, 2, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.HEARTLAND, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.CALIGA, 4, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.LAGRAS, 3, 0, 0));
        routes.Add(new Route(StationName.EMERALD, StationName.BUTCHER, 4, 0, 0));
        routes.Add(new Route(StationName.BUTCHER, StationName.VAN_HORN, 1, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.PRISON, 4, 0, 0));
        routes.Add(new Route(StationName.VAN_HORN, StationName.ST_DENIS, 5, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.STRAWBERRY, 1, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.BEECHER, 3, 0, 0));
        routes.Add(new Route(StationName.OWANJILA, StationName.MACFARLANE, 4, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.RIGGS, 2, 0, 0));
        routes.Add(new Route(StationName.STRAWBERRY, StationName.BEECHER, 3, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.FLATNECK, 2, 0, 0));
        routes.Add(new Route(StationName.RIGGS, StationName.BLACKWATER, 2, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.HEARTLAND, 3, 0, 0));
        routes.Add(new Route(StationName.FLATNECK, StationName.RHODES, 5, 0, 0));
        routes.Add(new Route(StationName.HEARTLAND, StationName.RHODES, 4, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.CALIGA, 2, 0, 0));
        routes.Add(new Route(StationName.LAGRAS, StationName.ST_DENIS, 3, 0, 0));
        routes.Add(new Route(StationName.PRISON, StationName.ST_DENIS, 3, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.BLACKWATER, 1, 0, 0));
        routes.Add(new Route(StationName.BEECHER, StationName.MACFARLANE, 3, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.RHODES, 6, 0, 0));
        routes.Add(new Route(StationName.BLACKWATER, StationName.THIEVES, 3, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.ST_DENIS, 4, 0, 0));
        routes.Add(new Route(StationName.RHODES, StationName.CALIGA, 2, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.CALIGA, 2, 0, 0));
        routes.Add(new Route(StationName.ST_DENIS, StationName.THIEVES, 8, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.TUMNBLEWEED, 2, 0, 0));
        routes.Add(new Route(StationName.COUJAR, StationName.BENEDICT, 2, 0, 0));
        routes.Add(new Route(StationName.TUMNBLEWEED, StationName.BENEDICT, 3, 0, 0));
        routes.Add(new Route(StationName.TUMNBLEWEED, StationName.MERCER, 4, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.DON_JILA, 2, 0, 0));
        routes.Add(new Route(StationName.MERCER, StationName.ARMADILLO, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.ARMADILLO, 2, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.MACFARLANE, 3, 0, 0));
        routes.Add(new Route(StationName.DON_JILA, StationName.THIEVES, 4, 0, 0));
        routes.Add(new Route(StationName.ARMADILLO, StationName.MACFARLANE, 3, 0, 0));
        routes.Add(new Route(StationName.MACFARLANE, StationName.THIEVES, 2, 0, 0));

        // read into map data
        foreach (Route route in routes)
        {
            if (!mapData.ContainsKey(route.Item1))
            {
                mapData.Add(route.Item1, new Node(route.Item1));
            }

            if (!mapData.ContainsKey(route.Item2))
            {
                mapData.Add(route.Item2, new Node(route.Item2));
            }

            mapData[route.Item1].AddNeighbor(route.Item2, route.Item3, route.Item4, route.Item5);
            mapData[route.Item2].AddNeighbor(route.Item1, route.Item3, route.Item4, route.Item5);
        }
    }

    public void ClearMapRoute()
    {
        foreach (Node node in mapData.Values)
        {
            node.ResetNode();
        }
    }

    public Node GetNodeByName(StationName stationName)
    {
        if (mapData.ContainsKey(stationName))
        {
            return mapData[stationName];
        }
        else
        {
            Debug.LogError("Station name not found in map data: " + stationName);
            return null;
        }
    }

    private List<Route> routes;
    private Dictionary<StationName, Node> mapData;

    private static GameDataCollection m_instance;
}