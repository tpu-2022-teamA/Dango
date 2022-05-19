using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitManager : MonoBehaviour
{
    [SerializeField] Player1 player;
    [SerializeField] private DangoUIScript DangoUISC;

    private void Awake()
    {
        if (DangoUISC == null)
        {
            DangoUISC = GameObject.Find("Canvas").transform.Find("DangoBackScreen").GetComponent<DangoUIScript>();
        }
    }

    /// <summary>
    /// 突き刺しボタンが押されたときにtrueになる。
    /// </summary>
    public bool isSticking = false;
    
    private void OnTriggerStay(Collider other)
    {
        //刺せる状態ではないなら実行しない
        if (!isSticking) return;

        if (other.gameObject.TryGetComponent(out DangoManager dango))
        {
            player.AddDangos(dango.GetDangoColor());
            DangoUISC.DangoUISet(player.GetDangos());

            other.gameObject.SetActive(false);
        }
    }
}
