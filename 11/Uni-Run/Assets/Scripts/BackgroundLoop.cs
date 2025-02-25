﻿using UnityEngine;

// 왼쪽 끝으로 이동한 배경을 오른쪽 끝으로 재배치하는 스크립트
public class BackgroundLoop : MonoBehaviour {
    private float width; // 배경의 가로 길이

    private void Awake()
    {
        var backGroundCollider = GetComponent<BoxCollider2D>();
        width = backGroundCollider.size.x;
    }

    private void Update() 
    {
        if (transform.position.x <= -width)
        {
            Reposition();
        }    
    }

    // 위치를 리셋하는 메서드
    private void Reposition()
    {
        var offset = new Vector2(width * 2f, 0);
        var transform1 = transform;
        transform1.position = (Vector2)transform1.position + offset;
    }
}