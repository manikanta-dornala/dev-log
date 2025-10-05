using UnityEngine;

[CreateAssetMenu(menuName = "Game Of Life/CreateNewPattern")]
public class Pattern : ScriptableObject
{
    public Vector2Int[] cells;

    public Vector2Int GetCenter()
    {
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;
        }

        Vector2Int min = cells[0];
        Vector2Int max = cells[0];

        for (int i = 1; i < cells.Length; i++)
        {
            min.x = Mathf.Min(cells[i].x, min.x);
            min.y = Mathf.Min(cells[i].y, min.y);
            max.x = Mathf.Max(cells[i].x, max.x);
            max.y = Mathf.Max(cells[i].y, max.y);
        }

        return (min + max) / 2;
    }
}