using UnityEngine;

[CreateAssetMenu(menuName = "Reel/ReelDataSO")]
public class ReelDataSO : ScriptableObject
{
    public ReelData[] reelDatas;

    public Material reelActiveMaterial;
    public Material reelDeactiveMaterial;
}
