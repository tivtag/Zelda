// --------------------------------------------------------------------------------------------- //
// SolidColor Effect
// --------------------------------------------------------------------------------------------- //
// Author:
//      Paul Ennemoser
// Version:
//      1.0
// Date:
//      8.13.07
// Description:
//     Fills any polygons drawn with it in a solid color.

uniform extern float4x4 WorldViewProj : WORLDVIEWPROJECTION;
 
struct VS_OUTPUT {
    float4 position : POSITION;
    float4 color    : COLOR0;
};
 
VS_OUTPUT MainVertexShader( float4 Pos   : POSITION, 
                        float4 Color : COLOR0   )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    Out.position = mul(Pos, WorldViewProj);
    Out.color    = Color;

    return Out;
}

float4 MainPixelShader( float4 Color : COLOR0 ) : COLOR0 
{
    return Color;
}

technique TransformTechnique {
    pass P0 {
        vertexShader     = compile vs_4_0 MainVertexShader();
        PixelShader      = compile ps_4_0 MainPixelShader();
        
        CullMode         = None;
        ZWriteEnable     = false;
        AlphaBlendEnable = true;
        SrcBlend         = SrcAlpha;
        DestBlend        = InvSrcAlpha;
    }
}
