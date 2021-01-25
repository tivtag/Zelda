//======================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//======================================================================
//
//  File: pp_HDR.fx
//
//  Desc: A few routines used for HDR tone mapping
//
//======================================================================

#include "Common.fxh"
#include "LogLuv.fxh"
#include "Tonemap.fxh"

float g_fDT;
float g_fBloomMultiplier;

float4 LuminancePS( in float2 in_vTexCoord : TEXCOORD0,
                    uniform bool bEncodeLogLuv ) : COLOR0
{
    float4 vSample = tex2D(LinearSampler0, in_vTexCoord);
    float3 vColor;
    if(bEncodeLogLuv)
       vColor = LogLuvDecode(vSample);
    else
       vColor = vSample.rgb;
   
    // calculate the luminance using a weighted average
    float fLuminance = dot(vColor, LUM_CONVERT);
                
    float fLogLuminace = log(1e-5 + fLuminance); 
        
    // Output the luminance to the render target
    return float4(fLogLuminace, 1.0f, 0.0f, 0.0f);
}

float4 CalcAdaptedLumPS (in float2 in_vTexCoord : TEXCOORD0) : COLOR0
{
    float fLastLum = tex2D(PointSampler1, float2(0.5f, 0.5f)).r;
    float fCurrentLum = tex2D(PointSampler0, float2(0.5f, 0.5f)).r;
    
    // Adapt the luminance using Pattanaik's technique
    const float fTau = 0.5f;
    float fAdaptedLum = fLastLum + (fCurrentLum - fLastLum) * (1 - exp(-g_fDT * fTau));
    
    return float4(fAdaptedLum, 1.0f, 1.0f, 1.0f);
}

float4 ToneMapPixelShader( in float2 in_vTexCoord : TEXCOORD0,
                           uniform bool bEncodeLogLuv ) : COLOR0
{
    // Sample the original HDR image
    float4 vSample = tex2D(PointSampler0, in_vTexCoord);
    float3 vHDRColor;
    if(bEncodeLogLuv)
        vHDRColor = LogLuvDecode(vSample);
    else
        vHDRColor = vSample.rgb;
    
    // Do the tone-mapping
    float3 vToneMapped = DoToneMap(vHDRColor);
    
    // Add in the bloom component
    float3 vColor = vToneMapped + tex2D(LinearSampler2, in_vTexCoord).rgb * g_fBloomMultiplier;
    
    return float4(vColor, 1.0f);
}

technique Luminance
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 LuminancePS(false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}

technique LuminanceEncode
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 LuminancePS(true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}

technique CalcAdaptedLuminance
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 CalcAdaptedLumPS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        StencilEnable = false;
    }
}

technique ToneMap
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 ToneMapPixelShader(false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
       // AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique ToneMapEncode
{
    pass p0
    {
        VertexShader = compile vs_4_0 PostProcessVS();
        PixelShader = compile ps_4_0 ToneMapPixelShader(true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        // AlphaTestEnable = false;
        StencilEnable = false;
    }
}
