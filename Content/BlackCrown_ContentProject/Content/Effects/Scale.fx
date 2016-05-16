//======================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//======================================================================
//
//	File:		pp_Scaling.fx
//
//	Desc:		Used for downscaling in software and scaling in
//				hardware.
//
//======================================================================

#include "Common.fxh"
#include "LogLuv.fxh"

static const float g_vOffsets[4] = {-1.5f, -0.5f, 0.5f, 1.5f};


// Downscales to 1/16 size, using 16 samples
float4 DownscalePS (	in float2 in_vTexCoord			: TEXCOORD0,
						uniform bool bEncodeLogLuv,
						uniform bool bDecodeLuminance	)	: COLOR0
{
	float4 vColor = 0;
	for (int x = 0; x < 4; x++)
	{
		for (int y = 0; y < 4; y++)
		{
			float2 vOffset;
			vOffset = float2(g_vOffsets[x], g_vOffsets[y]) / g_vSourceDimensions;
			float4 vSample = tex2D(PointSampler0, in_vTexCoord + vOffset);
			if (bEncodeLogLuv)
				vSample = float4(LogLuvDecode(vSample), 1.0f);
			vColor += vSample;
		}
	}

	vColor /= 16.0f;
	
	if (bEncodeLogLuv)
		vColor = LogLuvEncode(vColor.rgb);
		
	if (bDecodeLuminance)
		vColor = float4(exp(vColor.r), 1.0f, 1.0f, 1.0f);
	
	return vColor;
}

// Upscales or downscales using hardware bilinear filtering
float4 HWScalePS (	in float2 in_vTexCoord			: TEXCOORD0	)	: COLOR0
{
	return tex2D(LinearSampler0, in_vTexCoord);
}


technique Downscale4
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DownscalePS(false, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        //AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique Downscale4Luminance
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 DownscalePS(false, true);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
       // AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique Downscale4Encode
{
    pass p0
    {
        VertexShader = compile vs_3_0 PostProcessVS();
        PixelShader = compile ps_3_0 DownscalePS(true, false);
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
        //AlphaTestEnable = false;
        StencilEnable = false;
    }
}

technique ScaleHW
{
    pass p0
    {
        VertexShader = compile vs_2_0 PostProcessVS();
        PixelShader = compile ps_2_0 HWScalePS();
        
        ZEnable = false;
        ZWriteEnable = false;
        AlphaBlendEnable = false;
       // AlphaTestEnable = false;
        StencilEnable = false;
    }
}