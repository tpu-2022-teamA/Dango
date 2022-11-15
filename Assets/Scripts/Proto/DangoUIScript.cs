using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DangoUIScript : MonoBehaviour
{
    [SerializeField] GameObject[] Objs;
    [SerializeField] Sprite[] DangoImags;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] PlayerData PlayerData;
    private Image[] DangoImagObjs;

    private void Start()
    {
        DangoImagObjs = new Image[Objs.Length];
        for (int i = 0; i < Objs.Length; i++)
        {
            DangoImagObjs[i] = Objs[i].GetComponent<Image>();
            Objs[i].SetActive(false);
        }
    }
    private void Update()
    {
        Text.text = PlayerData.GetMaxDango().ToString();
    }
    public void DangoUISet(List<DangoColor> dangos)
    {
        for (int i = 0; i < dangos.Count; i++)
        {
            //団子の種類をみてマテリアルに色を付ける、画像が出来たらimagを切り替える。
            //団子が刺さっていないものがあれば非アクティブに
            Objs[i].SetActive(true);
            //DangoImagObjs[i].sprite = DangoImags[(int)dangos[i] - 1];
            DangoImagObjs[i].sprite = dangos[i] switch
            {
                DangoColor.Red => DangoImags[(int)DangoColor.Red],
                DangoColor.Orange => DangoImags[(int)DangoColor.Orange],
                DangoColor.Yellow => DangoImags[(int)DangoColor.Yellow],
                DangoColor.Green => DangoImags[(int)DangoColor.Green],
                DangoColor.Cyan => DangoImags[(int)DangoColor.Cyan],
                DangoColor.Blue => DangoImags[(int)DangoColor.Blue],
                DangoColor.Purple => DangoImags[(int)DangoColor.Purple],
                DangoColor.Other => DangoImags[(int)DangoColor.Other],
                _ => DangoImags[(int)DangoColor.Other],
            };
            Logger.Log((int)dangos[i]);
        }
        for (int i = dangos.Count; i < Objs.Length; i++)
        {
            Objs[i].SetActive(false);
        }
    }

}
