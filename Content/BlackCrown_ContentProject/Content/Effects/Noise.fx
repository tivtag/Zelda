// ***************************************
// *	    PERLIN NOISE SHADER			 *
// *									 *
// *								     *
// *								     *
// ***************************************


// ------- Parameters --------
Texture Texture;

float4 BaseColor   = float4( 0.1, 0.0, 0.0, 0.0 );
float4 ColorFactor = float4( 1.0, 1.0, 1.0, 0.15 );
float2 MoveOffset;
float  Overcast	   = 1.1f;


// ------- Texture Samplers --------
sampler TextureSampler = sampler_state
{ 
    texture   = <Texture>; 
    magfilter = LINEAR; 
    minfilter = LINEAR;
    mipfilter = LINEAR; 
    AddressU  = mirror; 
    AddressV  = mirror;
};


struct VertexShaderInputOuput
{
    float4 Position      : POSITION0;
    float2 TextureCoords : TEXCOORD;
};

VertexShaderInputOuput VertexShaderFunction(VertexShaderInputOuput input)
{
    return input;   
}


float4 PixelShaderFunction(VertexShaderInputOuput input) : COLOR0
{
     float4 perlin;     
     perlin  = tex2D(TextureSampler, (input.TextureCoords)      + MoveOffset) / 2;
     perlin += tex2D(TextureSampler, (input.TextureCoords) * 2  + MoveOffset) / 4;
     perlin += tex2D(TextureSampler, (input.TextureCoords) * 4  + MoveOffset) / 8;
     perlin += tex2D(TextureSampler, (input.TextureCoords) * 8  + MoveOffset) / 16;
     perlin += tex2D(TextureSampler, (input.TextureCoords) * 16 + MoveOffset) / 32;
     perlin += tex2D(TextureSampler, (input.TextureCoords) * 32 + MoveOffset) / 32; 

     float4 color = BaseColor + 1.0f - 2.0f * pow(perlin.r, Overcast);
     return color * ColorFactor;
}


technique PerlinNoiseTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader  = compile ps_4_0 PixelShaderFunction();
    }
}
