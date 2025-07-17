// ensure compatability using preprocessor commands.
#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 view_projection;

sampler TextureSampler : register(s0);

// we need samplers for both the main texture and noise texture to prevent the compiler getting trigger happy
Texture2D NoiseTexture;
sampler TextureSampler2 : register(s1)
{
    Texture = (NoiseTexture);
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float uTime;
float3 colour;
float scrollSpeed;
float opacity;
float maxWidth;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float2 ExtraData : TEXCOORD1;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float2 ExtraData : TEXCOORD1;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    //output.Position = mul(input.Position, view_projection);
    output.Position = input.Position;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    output.ExtraData = input.ExtraData;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{        
    // sample to avoid compiling out
    float2 uv = input.TextureCoordinates;
    float width = input.ExtraData.x;
    
    return float4(1 - width, 1 - width, 1 - width, 1);
    
    //uv.x = (uv.x - 0.5) / width + 0.5;
    
    float4 baseColor = tex2D(TextureSampler, uv).rgba;
    float dummy = baseColor.r * 0.001;

    float4 samp = NoiseTexture.Sample(TextureSampler2, uv);

    float4 white = float4(1, 1.0 - pow(dummy, 4), 1, 0);
    float4 black = float4(0, 0, 0, 0);
    float4 midColour = float4(colour.x, colour.y, colour.z, 0) * 0.6f;
    float finalOpacity = (1.0 - uv.y) * opacity;
    float maxWidthFrac = maxWidth * uv.y;
    
    float leftXForOutline = 0.1;
    float rightXForOutline = 0.9;
    
    if (uv.x < leftXForOutline)
    {
        return lerp(white, midColour, uv.x / leftXForOutline) * finalOpacity;
    }
    if (uv.x > rightXForOutline)
    {
        return lerp(white, midColour, (1 - uv.x) / (1 - rightXForOutline)) * finalOpacity;
    }
    
    return (samp + midColour) * finalOpacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}