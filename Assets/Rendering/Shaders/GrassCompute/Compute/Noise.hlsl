#ifndef CAPSTONE_NOISE
#define CAPSTONE_NOISE

// https://www.pcg-random.org/
uint pcg(uint v)
{
	uint state = v * 747796405u + 2891336453u;
	uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
	return (word >> 22u) ^ word;
}

int seed;
void SetupRandom(uint index) {
    seed = pcg(index);
}

float Random() {
    seed = pcg(seed);
    return abs(frac(float(seed) / 3141.592653));
}

float Random(float maxValue) {
    return Random() * maxValue;
}

float Random(float minValue, float maxValue) {
    return minValue + Random() * (maxValue - minValue);
}

#endif // End of File