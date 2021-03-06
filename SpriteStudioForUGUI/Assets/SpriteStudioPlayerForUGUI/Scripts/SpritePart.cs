﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using a.spritestudio.attribute;

namespace a.spritestudio
{
    /// <summary>
    /// パーツ
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent( typeof(RectTransform) )]
    [ExecuteInEditMode]
    public class SpritePart
        : MonoBehaviour
    {
        /// <summary>
        /// 親
        /// </summary>
        [SerializeField]
        private SpriteRoot root_;

        /// <summary>
        /// キーフレーム
        /// </summary>
        [SerializeField]
        private KeyFrame[] keyFrames_;

        /// <summary>
        /// NULLかどうか
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool isNull_;

        /// <summary>
        /// 描画
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private SpritePartRenderer renderer_;

        /// <summary>
        /// 前のフレーム
        /// </summary>
        private int oldFrame_;

        /// <summary>
        /// 更新有無
        /// </summary>
        private bool isUpdate_;

        /// <summary>
        /// 不透明度
        /// </summary>
        private float alpha_ = 1;

        /// <summary>
        /// 可視状態
        /// </summary>
        private bool isVisible_;

        /// <summary>
        /// GOの初期化時
        /// </summary>
        void Start()
        {
            oldFrame_ = -1;
            SetupVertices();
            if ( keyFrames_ != null && keyFrames_.Length > 0 ) {
                SetFrame( 0 );
            }
        }

        /// <summary>
        /// 親の取得
        /// </summary>
        public SpriteRoot Root
        {
            get { return root_; }
        }

        /// <summary>
        /// レンダラ
        /// </summary>
        public SpritePartRenderer Renderer
        {
            get { return renderer_; }
        }

        /// <summary>
        /// 優先度
        /// </summary>
        public int Priority
        {
            get { return renderer_.Priority; }
            set { renderer_.Priority = value; }
        }

        /// <summary>
        /// キーフレームセットの変更
        /// </summary>
        /// <param name="frames"></param>
        public void SetKeyFrames( KeyFrame[] frames )
        {
            keyFrames_ = frames;
            if ( frames != null && frames.Length > 0 ) {
                SetFrame( 0 );
            }
        }

        /// <summary>
        /// キーフレームセットの取得
        /// </summary>
        /// <returns></returns>
        public KeyFrame[] GetKeyFrames()
        {
            return keyFrames_;
        }

        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {
            if ( keyFrames_ == null || keyFrames_.Length == 0 ) { return; }

            // TODO: root側でするのがいいかも
            int frame = root_.CurrentFrame;
            if ( oldFrame_ != frame ) {
                SetFrame( frame );
            }

#if UNITY_EDITOR
            UpdateTransform();
#endif
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="root"></param>
        /// <param name="material"></param>
        public void Setup( SpriteRoot root, types.NodeType nodeType, Material material )
        {
            root_ = root;
            root.AddPart( this );

            isNull_ = nodeType == types.NodeType.kNull;
            if ( !isNull_ ) {
                // NULLノードでなければレンダラ生成
                var r = new GameObject( name, typeof( SpritePartRenderer ) );
                renderer_ = r.GetComponent<SpritePartRenderer>();
                renderer_.material = material;
                root.AddSprite( renderer_ );
                renderer_.Setup( this );
                SetupVertices();
            }

            keyFrames_ = new KeyFrame[root_.TotalFrames];
            for ( int i = 0; i < root_.TotalFrames; ++i ) {
                keyFrames_[i] = KeyFrame.Create();
            }
        }

        /// <summary>
        /// 頂点バッファ生成
        /// </summary>
        private void SetupVertices()
        {
            if ( renderer_ != null ) {
                renderer_.SetupVertices();
            }
        }

        /// <summary>
        /// 頂点変換
        /// </summary>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        public void TransformVertices( Vector2 leftTop, Vector2 rightTop, Vector2 leftBottom, Vector2 rightBottom )
        {
            if ( renderer_ != null ) {
                renderer_.TransformVertices( leftTop, rightTop, leftBottom, rightBottom );
            }
        }

        /// <summary>
        /// UV更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateTexCoordS( float value )
        {
            if ( renderer_ != null ) {
                renderer_.UpdateTexCoordS( value );
            }
        }

        /// <summary>
        /// UV更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateTexCoordT( float value )
        {
            if ( renderer_ != null ) {
                renderer_.UpdateTexCoordT( value );
            }
        }

        /// <summary>
        /// UV更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateTexCoordU( float value )
        {
            if ( renderer_ != null ) {
                renderer_.UpdateTexCoordU( value );
            }
        }

        /// <summary>
        /// UV更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateTexCoordV( float value )
        {
            if ( renderer_ != null ) {
                renderer_.UpdateTexCoordV( value );
            }
        }

        /// <summary>
        /// キーフレームの追加
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="attribute"></param>
        public void AddKey( int frame, AttributeBase attribute )
        {
            if ( frame >= keyFrames_.Length ) {
                // SSの不具合で範囲外のキーフレームが存在するので無視する
                Debug.LogWarning( "Key frame '" + attribute + "(" + frame + ")' is out of range in " + keyFrames_.Length );
                return;
            }
            keyFrames_[frame].Add( attribute );
        }

        /// <summary>
        /// キーフレームの設定
        /// </summary>
        /// <param name="frame"></param>
        public void SetFrame( int frame )
        {
            isUpdate_ = false;
            try {
                if ( root_.IsReverse ) {
                    // 逆再生
                    if ( oldFrame_ < frame ) {
                        oldFrame_ = keyFrames_.Length;
                    }
                    for ( int f = oldFrame_ - 1; f >= frame; --f ) {
                        KeyFrame attributes = keyFrames_[f];
                        isUpdate_ |= attributes.HasKey;
                        foreach ( var attribute in attributes ) {
                            attribute.Do( this );
                        }
                    }
                } else {
                    // 順再生
                    if ( oldFrame_ > frame ) {
                        oldFrame_ = -1;
                    }
                    for ( int f = oldFrame_ + 1; f <= frame; ++f ) {
                        KeyFrame attributes = keyFrames_[frame];
                        isUpdate_ |= attributes.HasKey;
                        foreach ( var attribute in attributes ) {
                            attribute.Do( this );
                        }
                    }
                }
            } catch {
                Debug.LogError( "error occurs at:" + keyFrames_.Length + "/" + frame );
                throw;
            }

            oldFrame_ = frame;
        }

        /// <summary>
        /// Transformの更新
        /// </summary>
        void LateUpdate()
        {
            if ( IsUpdate ) {
                if ( renderer_ != null ) {
                    renderer_.canvasRenderer.SetAlpha( Alpha );
                    renderer_.gameObject.SetActive( IsVisible );
                }
                UpdateTransform();
            }
        }

        /// <summary>
        /// 更新有無
        /// </summary>
        public bool IsUpdate
        {
            get
            {
                var parent = Parent;
                return isUpdate_ || (parent != null && parent.IsUpdate);
            }
        }

        /// <summary>
        /// 親ノード
        /// </summary>
        public SpritePart Parent
        {
            get
            {
                return transform.parent.GetComponent<SpritePart>();
            }
        }

        /// <summary>
        /// 不透明度
        /// </summary>
        public float Alpha
        {
            get
            {
                var parent = Parent;
                return alpha_ * (parent != null ? parent.Alpha : 1);
            }

            set
            {
                alpha_ = value;
            }
        }

        /// <summary>
        /// 可視状態
        /// </summary>
        public bool IsVisible
        {
            get
            {
                var parent = Parent;
                return isVisible_ && (parent == null || parent.IsVisible);
            }

            set
            {
                isVisible_ = value;
            }
        }

        /// <summary>
        /// セルマップの指定
        /// </summary>
        /// <param name="index"></param>
        /// <param name="mapIndex"></param>
        public void SetCellMap( int index, int mapIndex )
        {
            if ( renderer_ != null ) {
                renderer_.SetCellMap( index, mapIndex );
            }
        }

        /// <summary>
        /// Transformの更新
        /// </summary>
        private void UpdateTransform()
        {
            if ( renderer_ == null ) {return;}
            CopyTransform( GetComponent<RectTransform>(), renderer_.rectTransform );
        }

        /// <summary>
        /// Transformをコピー
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        private static void CopyTransform( RectTransform from, RectTransform to )
        {
            to.position = from.position;
            to.rotation = from.rotation;
            to.localScale = from.localScale;
        }
    }
}
