using UnityEngine;
using UnityEngine.EventSystems;

public class MoveInventory : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Canvas canvas;
    public int displayIndex;
    public int dragIndex = -1;

    // 이 스크립트를 가지고 있는 모든 오브젝트가 실행할 이벤트
    public static event System.Action<int, int> OnInventoryPositionChanged;

    public void Start()
    {
        canvas = GetComponent<Canvas>();

        // 모든 오브젝트가 실행할 함수를 입력
        OnInventoryPositionChanged += transform.parent.parent.GetComponent<InventroyPosition>().ChangePos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중일 때 UI 오브젝트의 위치 변경
        transform.position = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvas.overrideSorting = false;
        canvas.sortingOrder = 1;

        transform.localPosition = Vector3.zero;
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name.StartsWith("Slot"))
        {
            dragIndex = int.Parse(collision.name[4..]) - 1;

            // 이 스크립트를 달고 있는 모드 오브젝트가 ChangePos를 실행하도록 함
            if (displayIndex != dragIndex)
                OnInventoryPositionChanged(displayIndex, dragIndex);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (dragIndex != -1)
            return;

        OnInventoryPositionChanged(displayIndex, dragIndex);
        dragIndex = -1;
    }
}