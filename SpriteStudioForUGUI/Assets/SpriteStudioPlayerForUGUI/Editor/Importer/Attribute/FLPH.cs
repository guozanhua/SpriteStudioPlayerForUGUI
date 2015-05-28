﻿namespace a.spritestudio.editor.attribute
{
    public class FLPH
        : BasicBooleanAttribute
    {
        public override spritestudio.attribute.AttributeBase CreateKeyFrame( SpritePart part, ValueBase value )
        {
            Value v = (Value) value;
            return spritestudio.attribute.Flipper.Create( v.on, true );
        }
    }
}
