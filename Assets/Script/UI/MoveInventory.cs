using UnityEngine;
using UnityEngine.EventSystems;

public class MoveInventory : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Canvas canvas;
    public int displayIndex;
    public int dragIndex = -1;

    // �� ��ũ��Ʈ�� ������ �ִ� ��� ������Ʈ�� ������ �̺�Ʈ
    public static event System.Action<int, int> OnInventoryPositionChanged;

    public void Start()
    {
        canvas = GetComponent<Canvas>();

        // ��� ������Ʈ�� ������ �Լ��� �Է�
        OnInventoryPositionChanged += transform.parent.parent.GetComponent<InventroyPosition>().ChangePos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvas.overrideSorting = true;
        canvas.sortingOrder = 100;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� ���� �� UI ������Ʈ�� ��ġ ����
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

            // �� ��ũ��Ʈ�� �ް� �ִ� ��� ������Ʈ�� ChangePos�� �����ϵ��� ��
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