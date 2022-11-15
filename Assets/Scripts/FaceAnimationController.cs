using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フェイスアニメーション管理クラス.
/// </summary>
public class FaceAnimationController : MonoBehaviour
{
    public enum FaceTypes
    {
        Default,    /// デフォルト. 
        OpenMouth,  /// 口開け
        Chewing,    /// 咀嚼
        Smile,      /// ニコッ

        ForTitle,   /// タイトル用

        NoFace,     /// のっぺらぼう
    }

    /// <summary>
    /// アニメーション情報
    /// </summary>
    private struct AnimationInfo
    {
        public Vector2 TextureSize; /// テクスチャのサイズ. 
        public Rect Atlas;          /// アトラス情婦. 
        public int Type;            /// 種類. 
        public Material Mat;        /// マテリアル. 
        public int VNum;            /// 縦方向のアトラス数
        public int HNum;            /// 横方向のアトラス数
    }

    [SerializeField] Material faceMaterial;

    static readonly List<Vector2> _faceOffsetTable = new() { Vector2.zero, new(0.3282f, 0), new(0.6563f, 0), new(0, 0.4846f), new(0.3282f, 0.4846f), new(0.6563f, 0.4846f) };

    /// <summary>
    /// 顔の種類を変更.
    /// </summary>
    /// <param name="type">FaceTypes.</param>
    public void ChangeFaceType(FaceTypes type)
    {
        faceMaterial.SetTextureOffset("_MainTex", _faceOffsetTable[(int)type]);
    }
}
