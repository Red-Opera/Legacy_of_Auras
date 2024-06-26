using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMenu : MonoBehaviour
{
    public GameObject settingsUI;       // ������ �ʿ��� UI
    public GameObject[] settingMenu;    // ������ �޴�

    public GameObject menuUI;           // �޴��� UI
    public GameObject[] menuMenu;       // �޴��� �޴�

    private int nowMenuIndex = 0;       // ���� Ȱ��ȭ�Ǿ� �ִ� �޴��� �ε���

    public static bool isMenu = false;  // ���� Menu UI�� ���� �ִ��� ����
    private bool isSettings = false;    // ���� Settings UI�� ���� �ִ��� ����

    void Start()
    {
        Debug.Assert(settingsUI != null, "Error (Null Reference) : ���� �޴� ������Ʈ�� �������� �ʽ��ϴ�.");

        // ù��° �޴��� ������ ��� �޴��� ����
        for (int i = 1; i < settingMenu.Length; i++)
        {
            settingMenu[i].SetActive(false);
            menuMenu[i].SetActive(false);
        }
        
        // ���� UI�� ����
        settingsUI.SetActive(false);
        menuUI.SetActive(false);
    }

    void Update()
    {
        // ESC ��ư�� ���� ���
        if (Input.GetKeyDown(KeyCode.Escape) && !isMenu && !ItemShopOpenClose.isShopOpen)
        {
            isSettings = true;
            ClickESC();
        }

        // Q ��ư�� ���� ���
        if (Input.GetKeyDown(KeyCode.Q) && !isSettings)
        {
            isMenu = true;
            ClickQ();
        }
    }

    // ESC ��ư�� ���� �� �߻��ϴ� �޼ҵ�
    public void ClickESC()
    {
        if (ItemShopOpenClose.isShopOpen)
            return;

        if (!settingsUI.activeSelf)
            OpenMenu();

        else
            CloseMenu();
    }

    // ���� �޴� ��ư�� ������ ���
    public void ChangeSettingMenu()
    {
        // Ŭ���� ��ư�� �̸��� ������
        string clickName = EventSystem.current.currentSelectedGameObject.name;

        // �ڷΰ��� ��ư�� ���� ���
        if (clickName == "Return")
        {
            CloseMenu();
            return;
        }

        // ������ �޴��� �ƴ� �ٸ� �޴��� ���� ���
        if (settingMenu[nowMenuIndex].name != clickName)
        {
            // ������ ������ �޴��� ����
            settingMenu[nowMenuIndex].gameObject.SetActive(false);

            // ������ �޴��� ã��
            for (int i = 0; i < settingMenu.Length; i++)
            {
                if (settingMenu[i].name == clickName)
                {
                    nowMenuIndex = i;
                    break;
                }
            }

            // ������ �޴��� Ȱ��ȭ��
            settingMenu[nowMenuIndex].gameObject.SetActive(true);
        }
    }

    // Q ��ư�� ���� �� �߻��ϴ� �޼ҵ�
    public void ClickQ()
    {
        if (!menuUI.activeSelf)
            OpenMenu();

        else
            CloseMenu();
    }

    public void ChangeMenuMenu()
    {
        // Ŭ���� ��ư�� �̸��� ������
        string clickName = EventSystem.current.currentSelectedGameObject.name;

        // �ڷΰ��� ��ư�� ���� ���
        if (clickName == "Return")
        {
            CloseMenu();
            return;
        }

        // ������ �޴��� �ƴ� �ٸ� �޴��� ���� ���
        if (menuMenu[nowMenuIndex].name != clickName)
        {
            // ������ ������ �޴��� ����
            menuMenu[nowMenuIndex].gameObject.SetActive(false);

            // ������ �޴��� ã��
            for (int i = 0; i < menuMenu.Length; i++)
            {
                if (menuMenu[i].name == clickName)
                {
                    nowMenuIndex = i;
                    break;
                }
            }

            // ������ �޴��� Ȱ��ȭ��
            menuMenu[nowMenuIndex].gameObject.SetActive(true);
        }
    }

    private void OpenMenu()
    {
        Cursor.visible = true;                  // Ŀ���� �ٽ� ���̵��� ����
        Cursor.lockState = CursorLockMode.None; // Ŀ���� ������ �� �ֵ��� ����

        if (isSettings)
            settingsUI.SetActive(true);

        else 
            menuUI.SetActive(true);
    }

    private void CloseMenu()
    {
        Cursor.visible = false;                     // Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked;   // Ŀ���� �������� �ʵ��� ����
        
        if (settingsUI.activeSelf)
        {
            // �޴� �ʱ�ȭ
            settingMenu[nowMenuIndex].SetActive(false);
            settingMenu[0].SetActive(true);

            // UI ����
            settingsUI.SetActive(false);
            isSettings = false;
        }

        else
        {
            menuMenu[nowMenuIndex].SetActive(false);
            menuMenu[0].SetActive(true);

            menuUI.SetActive(false);
            isMenu = false;
        }

        nowMenuIndex = 0;
    }
}