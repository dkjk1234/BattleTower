using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject towerPrefab; // The prefab of the tower to instantiate
    private GameObject dragIcon;   // The visual representation of the tower during drag
    private RectTransform dragRectTransform;
    private Canvas canvas;
    
    private GameObject towerObj; // The Canvas that the UI elements are on
    private TowerRange towerRange;
    private SpriteRenderer towerRangeSr;

    private void Start()
    {
        // Find the canvas in the parent hierarchy
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        towerObj = Instantiate(towerPrefab, eventData.position, Quaternion.identity);
        towerRange = towerObj.GetComponentInChildren<TowerRange>();
        towerRangeSr = towerRange.GetComponent<SpriteRenderer>();



    }

    public void OnDrag(PointerEventData eventData)
    {

        if (towerObj != null)
        {
            Vector3 worldPosition = Vector3.zero;
            if (GetWorldPosition(eventData, out worldPosition))
            {

                var gridpos = LevelManager.Instance.WorldToGridPosition(worldPosition);
                towerObj.transform.position = LevelManager.Instance.GridToWorldPosition(gridpos) + new Vector3(0, 0.13f, 0); //+ new Vector3(-0.349000007f, 0.221f, 0);

                if (IsValidTowerPosition(worldPosition))
                {
                    Color successCol = Color.green;
                    successCol.a = 0.4f;
                    towerRangeSr.color = successCol;
                }
                else
                {
                    Color failCol = Color.red;
                    failCol.a = 0.4f;
                    towerRangeSr.color = failCol;
                } 
                /*if (!IsLoad())
                {
                    Color successCol = Color.green;
                    successCol.a = 0.4f;
                    towerRangeSr.color = successCol;
                }
                else
                {
                    Color failCol = Color.red;
                    failCol.a = 0.4f;
                    towerRangeSr.color = failCol;
                }*/

            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Enderag");
        // Remove the drag icon
        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }

        // Check if the drop position is valid for tower placement
        Vector3 worldPosition;
        if (GetWorldPosition(eventData, out worldPosition))
        {
            Debug.Log("PossibleBatch");
            if (IsValidTowerPosition(worldPosition))
            {
                
                // Instantiate the tower at the valid position
                
                Tower tower = towerObj.GetComponent<Tower>();
                tower.Setup(worldPosition);
                towerRangeSr.enabled = false;
                Initalized();
                return;
            }
        }
        Destroy(towerObj);
        Initalized();
    }

    private void Initalized()
    {
        towerObj = null;
        towerRangeSr = null;
        towerRangeSr = null;   
        
    }
    

    private bool GetWorldPosition(PointerEventData eventData, out Vector3 worldPosition)
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos , Vector2.zero);
        int layerMask = LayerMask.GetMask("GameField"); // Adjust the layer name as needed
        
        if (Physics2D.Raycast(mousePos , Vector2.zero, layerMask))
        {
   
            Debug.Log(hit.transform.name);
            worldPosition = new Vector3(hit.point.x, hit.point.y, 0); //-  new Vector3(-0.349000007f,0.0909999982f,0f) ;
            return true;
        }
        worldPosition = Vector3.zero;
        return false;
    }

    private bool IsValidTowerPosition(Vector3 position)
    {
        // Implement logic to check if the tower can be placed at the position
        
        Point gridPos = LevelManager.Instance.WorldToGridPosition(position);
        return LevelManager.Instance.IsValidTowerPosition(gridPos);
    }
}
