﻿using UnityEngine;
using System.Collections.Generic;

namespace a.spritestudio
{
    /// <summary>
    /// キーフレーム
    /// </summary>
    [System.Serializable]
    public class KeyFrame
    {
        /// <summary>
        /// 
        /// </summary>
        [SerializeField]
        private List<attribute.AttributeBase> attributes_;

        /// <summary>
        /// 
        /// </summary>
        private KeyFrame() { }

        /// <summary>
        /// 
        /// </summary>
        public static KeyFrame Create()
        {
            KeyFrame self = new KeyFrame();
            self.attributes_ = new List<attribute.AttributeBase>();
            return self;
        }

        /// <summary>
        /// キーが1つ以上あるか
        /// </summary>
        public bool HasKey
        {
            get { return attributes_ != null && attributes_.Count > 0; }
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="attribute"></param>
        public void Add( attribute.AttributeBase attribute )
        {
            attributes_.Add( attribute );
        }

        /// <summary>
        /// for-each用
        /// </summary>
        /// <returns></returns>
        public IEnumerator<attribute.AttributeBase> GetEnumerator()
        {
            return attributes_.GetEnumerator();
        }
    }
}
