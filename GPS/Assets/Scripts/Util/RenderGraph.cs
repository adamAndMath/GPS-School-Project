using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RenderGraph : MonoBehaviour
{
    public Material mat;
    public Color graphColor;
    public Vector2 size;
    public Vector2 splitPer;
    public List<Vector2> points;

	void OnPostRender()
    {
        GL.PushMatrix();
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Begin(GL.LINES);

        for (var i = 0F; i < size.x; i += splitPer.x)
        {
            GL.Vertex(new Vector2(i / size.x, 0));
            GL.Vertex(new Vector2(i / size.x, 1));
        }

        GL.Vertex(Vector2.up);
        GL.Vertex(Vector2.one);

        for (var i = 0F; i < size.y; i += splitPer.y)
        {
            GL.Vertex(new Vector2(0, i / size.y));
            GL.Vertex(new Vector2(1, i / size.y));
        }

        GL.Vertex(Vector2.right);
        GL.Vertex(Vector2.one);

        GL.Color(graphColor);

        for (var i = 0; i < points.Count - 1; )
        {
            GL.Vertex(points[i]);
            GL.Vertex(points[++i]);
        }

        GL.End();
        GL.PopMatrix();
    }
}
