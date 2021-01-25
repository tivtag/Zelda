//======================================================================
//
//	HDRSample
//
//		by MJP
//		09/20/08
//
//======================================================================
//
//	File:		LogLuv.fxh
//
//	Desc:		Contains functions for encoding and decoding the
//				the LogLuv format, based on Marco Salvi's "NAO32"
//				technique used for Heavenly Sword.  Mad props go to
//				Christer Ericson for posting an optimized version of
//				the encoding technique on his blog.
//				(http://realtimecollisiondetection.net/blog/?p=15).
//				Equally mad props go to remigius for helping me fix
//				the banding.
//
//======================================================================

// M matrix, for encoding
const static float3x3 M = float3x3(
    0.2209, 0.3390, 0.4184,
    0.1138, 0.6780, 0.7319,
    0.0102, 0.1130, 0.2969);

// Inverse M matrix, for decoding
const static float3x3 InverseM = float3x3(
     6.0013, -2.700,  -1.7995,
    -1.332,   3.1029, -5.7720,
     .3007,  -1.088,   5.6268);

float4 LogLuvEncode(in float3 vRGB) 
{
    float4 vResult; 
    float3 Xp_Y_XYZp = mul(vRGB, M);
    Xp_Y_XYZp = max(Xp_Y_XYZp, float3(1e-6, 1e-6, 1e-6));
    vResult.xy = Xp_Y_XYZp.xy / Xp_Y_XYZp.z;
    float Le = 2 * log2(Xp_Y_XYZp.y) + 127;
    vResult.w = frac(Le);
    vResult.z = (Le - (floor(vResult.w*255.0f))/255.0f)/255.0f;
    return vResult;
}

float3 LogLuvDecode(in float4 vLogLuv)
{
    float Le = vLogLuv.z * 255 + vLogLuv.w;
    float3 Xp_Y_XYZp;
    Xp_Y_XYZp.y = exp2((Le - 127) / 2);
    Xp_Y_XYZp.z = Xp_Y_XYZp.y / vLogLuv.y;
    Xp_Y_XYZp.x = vLogLuv.x * Xp_Y_XYZp.z;
    float3 vRGB = mul(Xp_Y_XYZp, InverseM);
    return max(vRGB, 0);
}
