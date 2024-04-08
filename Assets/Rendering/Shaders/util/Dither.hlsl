#ifndef CAPSTONE_UTIL_DITHER
#define CAPSTONE_UTIL_DITHER

float Dither(float2 pixelPos) {
    static const float g = 1.32471795724474602596f;
    static const float a1 = 1/g;
    static const float a2 = 1/(g*g);
    return fmod(pixelPos.x*a1 + pixelPos.y*a2, 1.0f);
}

float2 Dither2(float2 pixelPos) {
    return float2(Dither(pixelPos), Dither(pixelPos.yx + 0.5));
}

#endif