#region Using Statements
using System;
#endregion

namespace MarkUX.Animation
{
    /// <summary>
    /// Specifies what kind of easing function should be used when values are interpolated.
    /// </summary>
    public enum EasingFunctionType
    {
        Linear = 0,

        QuadraticEaseIn = 1, 
        QuadraticEaseOut = 2,         
        QuadraticEaseInOut = 3,

        CubicEaseIn = 4,
        CubicEaseOut = 5,        
        CubicEaseInOut = 6,

        QuarticEaseIn = 7,
        QuarticEaseOut = 8,        
        QuarticEaseInOut = 9,

        QuinticEaseIn = 10,
        QuinticEaseOut = 11,        
        QuinticEaseInOut = 12,
                
        SineEaseIn = 13,
        SineEaseOut = 14,
        SineEaseInOut = 15,

        CircularEaseIn = 16,
        CircularEaseOut = 17,        
        CircularEaseInOut = 18,

        ExponentialEaseIn = 19, 
        ExponentialEaseOut = 20,         
        ExponentialEaseInOut = 21,

        ElasticEaseIn = 22, 
        ElasticEaseOut = 23,         
        ElasticEaseInOut = 24,

        BounceEaseIn = 25, 
        BounceEaseOut = 26,         
        BounceEaseInOut = 27,

        BackEaseIn = 28, 
        BackEaseOut = 29,        
        BackEaseInOut = 30
    }
}