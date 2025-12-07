using UnityEngine;

[CreateAssetMenu(fileName = "NewEndingData", menuName = "Ending System/Ending Data")]
public class EndingData : ScriptableObject
{
    [Header("엔딩 설정")]
    public EndingType endingType; // 엔딩 종류 (Enum)
    public Sprite backgroundImage; // 배경 풍경 사진

    [Header("나레이션 텍스트")]
    [TextArea(3, 10)]
    public string[] narrationLines; // 대사 목록
}

// 엔딩 종류 정의
public enum EndingType
{
    Normal,
    Happy,
    Bad
}