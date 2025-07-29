// ensure compatability using preprocessor commands.
#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 worldViewProjection;

sampler TextureSampler : register(s0);

// we need samplers for both the main texture and noise texture to prevent the compiler getting trigger happy
Texture2D NoiseTexture;
sampler NoiseSampler : register(s1)
{
    Texture = (NoiseTexture);
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float uTime;
float fadeOutProgress;
float3 colour;
float scrollSpeed;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
    // z component used for arbitrary extra byte like width
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Position = mul(input.Position, worldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float easeInExpo(float x)
{
    return x == 0 ? 0 : pow(2, 10 * x - 10);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates.xy; // make x go along the trail
    uv.x = input.TextureCoordinates.z;
    
    //float value = lerp(0, 0.3, uv.y > 0.5);
    //return float4(value + colour.x, value + colour.y, value + colour.z, 1);
    
    float4 white = float4(1, 1, 1, 0);
    float4 black = float4(0, 0, 0, 0);
    
    float4 baseColor = tex2D(TextureSampler, uv).rgba;

    float opacity = 1 - fadeOutProgress + baseColor.r / 1000;
    
    float4 samp = NoiseTexture.Sample(NoiseSampler, uv);

    // figure out how close we are to being cutoff - if we're close but not cut off, be white
    float whiteness = uv.x * 0.1;
    return float4(colour.x + whiteness, colour.y + whiteness, colour.z + whiteness, 1);
}

Technique Technique1
{
    pass ShaderPass
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}