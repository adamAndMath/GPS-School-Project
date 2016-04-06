using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Old
{
    [CustomEditor(typeof(Node)), CanEditMultipleObjects]
    public class NodeEditor : Editor
    {
        private Node roadFrom;

        void OnSceneGUI()
        {
            var node = (Node)target;
            var screenPos = Camera.current.WorldToScreenPoint(node.transform.position);
            var rect = new Rect(screenPos.x, Screen.height - screenPos.y - 38, 16, 16);

            Handles.BeginGUI();

            if (roadFrom == null)
            {
                if (GUI.Button(rect, "+"))
                    roadFrom = node;
            }
            else if (roadFrom == node)
            {
                if (GUI.Button(rect, "C"))
                    roadFrom = null;
            }
            else if (roadFrom.roads.All(r => r.to != node))
            {
                if (GUI.Button(rect, "+"))
                {
                    var road = Instantiate(roadFrom.roadPrefab.gameObject).GetComponent<Road>();
                    road.Init(roadFrom, node);
                    roadFrom.roads.Add(road);
                    roadFrom = null;
                }
            }

            Handles.EndGUI();
        }
    }
}