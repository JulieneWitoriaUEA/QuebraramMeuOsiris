using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BoxGravity : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform phaseTransform;
    [SerializeField] private Collider2D smashCollider;

    [Header("Box Settings")]
    [SerializeField] private Vector2Int boxSize = new Vector2Int(1, 1);
    [SerializeField] private Vector3 pivotOffset = Vector3.zero;

    [Header("Gravity")]
    [SerializeField] private float fallSpeed = 2f;

    [Header("Obstáculos")]
    [SerializeField] private LayerMask obstacleMask;

    private bool isFalling = false;
    private Vector3Int gravityDir;

    private void Update()
    {
        if (TryFall())
        {
            if (!isFalling) Debug.Log($"{name} começou a cair");
            isFalling = true;
            smashCollider.enabled = false;
            transform.localPosition += (Vector3)gravityDir * fallSpeed * Time.deltaTime;
        }
        else
        {
            if (isFalling)
            {
                Debug.Log($"{name} parou de cair, snapando no grid");
                SnapToGrid();
            }
            isFalling = false;
            smashCollider.enabled = true;
        }
    }
    private bool TryFall()
    {
        // Direção da gravidade (garantido cardeal)
        gravityDir = Vector3Int.RoundToInt(-phaseTransform.up.normalized);
        gravityDir.x = -Mathf.Clamp(gravityDir.x, -1, 1);
        gravityDir.y = Mathf.Clamp(gravityDir.y, -1, 1);
        gravityDir.z = 0;
        Debug.Log($"{name} gravityDir: {gravityDir}");

        // origem baseada no PIVOT (offset local → world)
        Vector3 pivotWorld = transform.TransformPoint(pivotOffset);
        Vector3Int originCell = tilemap.WorldToCell(pivotWorld);

        // padrão de sinais separado + deslocamento do canto de origem
        int signX = (gravityDir.x != 0) ? -gravityDir.x : 1; // direita => -1, esquerda => +1, vertical => +1
        int signY = (gravityDir.y != 0) ? -gravityDir.y : 1; // cima => -1, baixo => +1, horizontal => +1

        Vector3Int originShift = new Vector3Int(
            gravityDir.x == 1 ? boxSize.x - 1 : 0, // se gravidade → direita, começa no canto direito
            gravityDir.y == 1 ? boxSize.y - 1 : 0, // se gravidade → cima, começa no canto superior
            0
        );

        List<Vector3Int> baseCells = new List<Vector3Int>();
        for (int x = 0; x < boxSize.x; x++)
        {
            for (int y = 0; y < boxSize.y; y++)
            {
                Vector3Int c = originCell + originShift + new Vector3Int(signX * x, signY * y, 0);

                // base = faixa 0 no eixo da gravidade
                if (gravityDir.y != 0) { if (y == 0) baseCells.Add(c); } // gravidade vertical → usa linha y==0
                else { if (x == 0) baseCells.Add(c); } // gravidade horizontal → usa coluna x==0
            }
        }


        // Debug das baseCells
        foreach (var bc in baseCells)
            Debug.Log($"{name} baseCell: {bc}");

        // Testa 1 célula além da base
        foreach (var c in baseCells)
        {
            Vector3Int check = c + gravityDir;
            if (HasObstacleAtCell(check))
            {
                Debug.Log($"{name} bloqueada por tile em {check}");
                return false;
            }
        }
        return true;
    }

    private bool HasObstacleAtCell(Vector3Int cell)
    {
        Vector3 center = tilemap.GetCellCenterWorld(cell);
        Vector2 cellSize = new Vector2(Mathf.Abs(tilemap.cellSize.x), Mathf.Abs(tilemap.cellSize.y));
        if (cellSize == Vector2.zero) cellSize = Vector2.one; // fallback se cellSize não estiver configurado
        Vector2 boxSize = cellSize * 0.9f; // 90% do tile

        // Pega colisores no tile (layers de obstacleMask) e IGNORA triggers
        var hits = Physics2D.OverlapBoxAll(center, boxSize, 0f, obstacleMask);
        foreach (var h in hits)
        {
            if (h && !h.isTrigger) return true;
        }
        return false;
    }

    private void SnapToGrid()
    {
        Vector3Int cell = tilemap.WorldToCell(transform.position + pivotOffset);
        Vector3 snapped = tilemap.GetCellCenterWorld(cell);
        transform.position = new Vector3(snapped.x, snapped.y, transform.position.z) - pivotOffset;
    }
}
