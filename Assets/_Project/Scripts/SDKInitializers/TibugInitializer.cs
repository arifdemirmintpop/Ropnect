using UnityEngine;
using System.Collections;
using tiplay;

public class TibugInitializer : MonoBehaviour
{
#if TIPLAY_TIBUG
    private void Awake()
    {
        if (!GlobalData.TibugPrefab)
            return;

        if (FindObjectOfType<TibugManager>())
            return;

        Instantiate(GlobalData.TibugPrefab);
    }
#endif
}

