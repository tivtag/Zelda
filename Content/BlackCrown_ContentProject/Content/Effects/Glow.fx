texture inputTexture;
sampler inputSampler : register(s0);

texture GlowMap;
sampler GlowSampler = sampler_state {
	Texture = (GlowMap);
	ADDRESSU = CLAMP;
	ADDRESSV = CLAMP;
	MAGFILTER = LINEAR;
	MINFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


float GlowStrength;
float2 TexelSize;

static const int KernelSize = 13;

static const float2 PixelKernelH[KernelSize] = {
    {-6, 0},
    {-5, 0},
    {-4, 0},
    {-3, 0},
    {-2, 0},
    {-1, 0},
    { 0, 0},
    { 1, 0},
    { 2, 0},
    { 3, 0},
    { 4, 0},
    { 5, 0},
    { 6, 0},
};

static const float2 PixelKernelV[KernelSize] = {
    {0, -6},
    {0, -5},
    {0, -4},
    {0, -3},
    {0, -2},
    {0, -1},
    {0,  0},
    {0,  1},
    {0,  2},
    {0,  3},
    {0,  4},
    {0,  5},
    {0,  6},
};

static const float BlurWeights[KernelSize] = {
    0.002216,
    0.008764,
    0.026995,
    0.064759,
    0.120985,
    0.176033,
    0.199471,
    0.176033,
    0.120985,
    0.064759,
    0.026995,
    0.008764,
    0.002216,
};

float4 BlurHorizontalPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 Color = 0;
    for (int i = 0; i < KernelSize; i++)
    {    
        Color += tex2D( inputSampler, texCoord + PixelKernelH[i].xy * TexelSize) * BlurWeights[i];
    }
    return Color;
}

float4 BlurVerticalPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 Color = 0;

    for (int i = 0; i < KernelSize; i++)
    {    
        Color += tex2D( inputSampler, texCoord + PixelKernelV[i].xy * TexelSize) * BlurWeights[i];
    }
    return Color;
}

float4 CombinePS(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 Color = tex2D(inputSampler, texCoord);
	Color +=GlowStrength * tex2D(GlowSampler, texCoord);
    return Color;
}

float4 CopyPS(float2 texCoord : TEXCOORD0) : COLOR0
{
	return tex2D(inputSampler, texCoord);
}

technique BlurHorizontal
{
    pass P0
    {
        PixelShader = compile ps_2_0 BlurHorizontalPS();
    }
}
    
technique BlurVertical
{
    pass P0
    {
        PixelShader = compile ps_2_0 BlurVerticalPS();
    }
}

technique Combine
{
    pass P0
    {
        PixelShader = compile ps_2_0 CombinePS();
    }
}

technique Copy
{
    pass P0
    {
        PixelShader = compile ps_2_0 CopyPS();
    }
}