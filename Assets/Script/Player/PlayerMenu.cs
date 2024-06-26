using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMenu : MonoBehaviour
{
    public GameObject settingsUI;       // 설정에 필요한 UI
    public GameObject[] settingMenu;    // 설정의 메뉴

    public GameObject menuUI;           // 메뉴의 UI
    public GameObject[] menuMenu;       // 메뉴의 메뉴

    private int nowMenuIndex = 0;       // 현재 활성화되어 있는 메뉴의 인덱스

    public static bool isMenu = false;  // 현재 Menu UI가 열려 있는지 여부
    private bool isSettings = false;    // 현재 Settings UI가 열려 있는지 여부

    void Start()
    {
        Debug.Assert(settingsUI != null, "Error (Null Reference) : 설정 메뉴 오브젝트가 존재하지 않습니다.");

        // 첫번째 메뉴를 제외한 모든 메뉴는 숨김
        for (int i = 1; i < settingMenu.Length; i++)
        {
            settingMenu[i].SetActive(false);
            menuMenu[i].SetActive(false);
        }
        
        // 설정 UI를 숨김
        settingsUI.SetActive(false);
        menuUI.SetActive(false);
    }

    void Update()
    {
        // ESC 버튼을 누를 경우
        if (Input.GetKeyDown(KeyCode.Escape) && !isMenu && !ItemShopOpenClose.isShopOpen)
        {
            isSettings = true;
            ClickESC();
        }

        // Q 버튼을 누를 경우
        if (Input.GetKeyDown(KeyCode.Q) && !isSettings)
        {
            isMenu = true;
            ClickQ();
        }
    }

    // ESC 버튼을 누를 때 발생하는 메소드
    public void ClickESC()
    {
        if (ItemShopOpenClose.isShopOpen)
            return;

        if (!settingsUI.activeSelf)
            OpenMenu();

        else
            CloseMenu();
    }

    // 설정 메뉴 버튼을 누렸을 경우
    public void ChangeSettingMenu()
    {
        // 클릭한 버튼의 이름을 가져옴
        string clickName = EventSystem.current.currentSelectedGameObject.name;

        // 뒤로가기 버튼을 누른 경우
        if (clickName == "Return")
        {
            CloseMenu();
            return;
        }

        // 선택한 메뉴가 아닌 다른 메뉴를 누른 경우
        if (settingMenu[nowMenuIndex].name != clickName)
        {
            // 이전에 선택한 메뉴는 숨김
            settingMenu[nowMenuIndex].gameObject.SetActive(false);

            // 선택한 메뉴를 찾음
            for (int i = 0; i < settingMenu.Length; i++)
            {
                if (settingMenu[i].name == clickName)
                {
                    nowMenuIndex = i;
                    break;
                }
            }

            // 선택한 메뉴를 활성화함
            settingMenu[nowMenuIndex].gameObject.SetActive(true);
        }
    }

    // Q 버튼을 누를 때 발생하는 메소드
    public void ClickQ()
    {
        if (!menuUI.activeSelf)
            OpenMenu();

        else
            CloseMenu();
    }

    public void ChangeMenuMenu()
    {
        // 클릭한 버튼의 이름을 가져옴
        string clickName = EventSystem.current.currentSelectedGameObject.name;

        // 뒤로가기 버튼을 누른 경우
        if (clickName == "Return")
        {
            CloseMenu();
            return;
        }

        // 선택한 메뉴가 아닌 다른 메뉴를 누른 경우
        if (menuMenu[nowMenuIndex].name != clickName)
        {
            // 이전에 선택한 메뉴는 숨김
            menuMenu[nowMenuIndex].gameObject.SetActive(false);

            // 선택한 메뉴를 찾음
            for (int i = 0; i < menuMenu.Length; i++)
            {
                if (menuMenu[i].name == clickName)
                {
                    nowMenuIndex = i;
                    break;
                }
            }

            // 선택한 메뉴를 활성화함
            menuMenu[nowMenuIndex].gameObject.SetActive(true);
        }
    }

    private void OpenMenu()
    {
        Cursor.visible = true;                  // 커서를 다시 보이도록 설정
        Cursor.lockState = CursorLockMode.None; // 커서가 움직일 수 있도록 설정

        if (isSettings)
            settingsUI.SetActive(true);

        else 
            menuUI.SetActive(true);
    }

    private void CloseMenu()
    {
        Cursor.visible = false;                     // 커서를 감춤
        Cursor.lockState = CursorLockMode.Locked;   // 커서가 움직이지 않도록 설정
        
        if (settingsUI.activeSelf)
        {
            // 메뉴 초기화
            settingMenu[nowMenuIndex].SetActive(false);
            settingMenu[0].SetActive(true);

            // UI 숨김
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