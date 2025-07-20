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
    float2 uv = input.TextureCoordinates.xy;
    
    float4 baseColor = tex2D(TextureSampler, uv).rgba;

    float opacity = 1 - fadeOutProgress + baseColor.r / 1000;
    
    float4 samp = NoiseTexture.Sample(NoiseSampler, uv);
    
    // use a certain grayness in the noise texture as the value from which we are brightest.
    float bestGrey = frac(uTime / 20);
    float closenessToBest = (uv.x + 0.7) * frac(samp.r + bestGrey);
    float taperOffFactor = easeInExpo(uv.y);
    opacity = pow(closenessToBest, 6) * taperOffFactor;
    
    float distanceToEdge = 1 - uv.x;
    float3 whiteness = 0.07 / distanceToEdge * smoothstep(float3(0, 0, 0), float3(1, 1, 1), distanceToEdge);
    
    float3 outColour = (colour + whiteness) * opacity;
    return float4(outColour, 0);
}

Technique Technique1
{
    pass ShaderPass
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}