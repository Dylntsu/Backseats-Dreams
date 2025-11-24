using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[AddComponentMenu("UI/Effects/UIGradient")]
public class UIGradient : BaseMeshEffect
{
    public Color colorLeft = Color.magenta;
    public Color colorRight = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        List<UIVertex> vertexList = new List<UIVertex>();
        vh.GetUIVertexStream(vertexList);

        int count = vertexList.Count;
        if (count == 0) return;

        // Encontramos los bordes
        float minX = vertexList[0].position.x;
        float maxX = vertexList[0].position.x;

        for (int i = 1; i < count; i++)
        {
            float x = vertexList[i].position.x;
            if (x > maxX) maxX = x;
            if (x < minX) minX = x;
        }

        float width = maxX - minX;

        // Aplicamos el color según la posición
        for (int i = 0; i < count; i++)
        {
            UIVertex v = vertexList[i];
            // Calcula el porcentaje (0 a 1) de izquierda a derecha
            float t = (width == 0) ? 0 : (v.position.x - minX) / width;

            // Mezcla los colores
            v.color = Color.Lerp(colorLeft, colorRight, t);
            vertexList[i] = v;
        }

        vh.Clear();
        vh.AddUIVertexTriangleStream(vertexList);
    }
}