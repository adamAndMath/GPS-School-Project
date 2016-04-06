using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Grapher : MonoBehaviour
{
    public Material mat;
    public Color graphColor;
    public RenderTexture renderTexture;
    public List<Vector2> points;

    private Image image;
    private Camera cam;
    private new RenderGraph renderer;

    public RenderGraph Renderer
    {
        get
        {
            if (!renderer)
            {
                var obj = new GameObject(name + "_Renderer", typeof(Camera), typeof(RenderGraph));
                obj.hideFlags = HideFlags.HideAndDontSave;
                cam = obj.GetComponent<Camera>();
                cam.cullingMask = 0;
                renderer = obj.GetComponent<RenderGraph>();
            }

            return renderer;
        }
    }
    public Vector2 Size { get { return Renderer.size; } set { Renderer.size = value; } }
    public Vector2 SplitPer { get { return Renderer.splitPer; } set { Renderer.splitPer = value; } }

	void Update ()
    {
        if (!image)
        {
            var obj = new GameObject(name + "_Image", typeof(Image));
            obj.transform.SetParent(transform, false);
            obj.hideFlags = HideFlags.HideAndDontSave;
            image = obj.GetComponent<Image>();
            image.sprite = Sprite.Create(new Texture2D(renderTexture.width, renderTexture.height), new Rect(Vector2.zero, new Vector2(renderTexture.width, renderTexture.height)), Vector2.zero);
            image.sprite.hideFlags = HideFlags.DontSave;
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.one;
            image.rectTransform.offsetMin = Vector2.zero;
            image.rectTransform.offsetMax = Vector2.zero;
        }

        Renderer.mat = mat;
        Renderer.graphColor = graphColor;
        Renderer.points = points;
        cam.targetTexture = renderTexture;

        if (image.sprite.texture.width != renderTexture.width || image.sprite.texture.height != renderTexture.height)
        {
            image.sprite.texture.Resize(renderTexture.width, renderTexture.height);
        }

        var currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        image.sprite.texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.sprite.texture.Apply();
        RenderTexture.active = currentRT;
        
	}
}
