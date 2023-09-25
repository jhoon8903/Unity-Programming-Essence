using UnityEngine;

// 발판으로서 필요한 동작을 담은 스크립트
public class Platform : MonoBehaviour 
{
    public GameObject[] obstacles; // 장애물 오브젝트들
    private bool stepped = false; // 플레이어 캐릭터가 밟았었는가

    // 컴포넌트가 활성화될때 마다 매번 실행되는 메서드
    private void OnEnable()
    {
        stepped = false;
        foreach (var t in obstacles)
        {
            t.SetActive(Random.Range(0, 3) == 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.collider.CompareTag("Player") && !stepped)
        {
            stepped = true;
            GameManager.instance.AddScore(1);
        }
    }
}