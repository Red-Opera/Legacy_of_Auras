using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public void Awake()
    {
        Cursor.visible = false;                     // Ŀ���� ����
        Cursor.lockState = CursorLockMode.Locked;   // Ŀ���� �������� �ʵ��� ����
    }

    public void Update()
    {
        
    }
}
