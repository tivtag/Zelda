float4x4 World;
float4x4 View;
float4x4 Projection;

float3 StartColor;
float3 EndColor;
float FallOffPower = 1.0f;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 ColorGradient : TEXCOORD1;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 ColorGradient : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
    output.ColorGradient = input.ColorGradient;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float p = saturate(length(input.ColorGradient));
    
    float lerp_factor = saturate(pow(1-p, FallOffPower));
    float brightness = lerp_factor;
    float3 color = lerp(EndColor,StartColor,lerp_factor);
    return brightness * float4(color,1.0f);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }
}
