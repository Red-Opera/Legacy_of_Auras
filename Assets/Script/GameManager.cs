using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Awake()
    {
        Cursor.visible = false;                     // 커서를 감춤
        Cursor.lockState = CursorLockMode.Locked;   // 커서가 움직이지 않도록 설정
    }

    public void Update()
    {
        
    }
}
