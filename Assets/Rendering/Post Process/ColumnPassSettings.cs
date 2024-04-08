using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(ColumnPassRenderer), PostProcessEvent.BeforeStack, "Transition Column")]
public class ColumnPassSettings : PostProcessEffectSettings
{
    public FloatParameter densityFacing = new FloatParameter { value = 0.25f };
    public FloatParameter speed = new FloatParameter { value = 5f };
    public FloatParameter distortionSize = new FloatParameter { value = 1f };
    public FloatParameter distortionStretch = new FloatParameter { value = 10f };
    public FloatParameter distortionStrength = new FloatParameter { value = 1f };

    public IntParameter thinOctave = new IntParameter { value = 3 };
    public FloatParameter thinDensity = new FloatParameter { value = 0.5f };
}
