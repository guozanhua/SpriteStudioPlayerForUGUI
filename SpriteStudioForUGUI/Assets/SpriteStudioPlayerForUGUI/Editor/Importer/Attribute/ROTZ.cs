﻿using a.spritestudio.attribute;

namespace a.spritestudio.editor.attribute
{
    public class ROTZ
        : BasicSingleFloatAttribute
    {
        public override AttributeBase CreateKeyFrame( SpritePart part, ValueBase value )
        {
            Value v = (Value) value;
            return RotationUpdater.Create( RotationUpdater.kTargetZ, v.value );
        }
    }
}
