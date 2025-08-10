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
    //uv.x = input.TextureCoordinates.z;
    
    //float value = lerp(0, 0.3, uv.x > 0.9);
    //return float4(value + colour.x, value + colour.y, value + colour.z, 1);
    
    //return float4(uv.xxx, 1);
    
    //return float4(1, 1, 1, 1);
    
    float4 white = float4(1, 1, 1, 0);
    
    float4 baseColor = tex2D(TextureSampler, uv).rgba;

    float opacity = 1 + baseColor.r * 0.001;
    
    float2 scrollAmount = float2(0.005 * uTime, 0);
    
    float4 samp = NoiseTexture.Sample(NoiseSampler, uv - scrollAmount);
    float whiteThreshold = 0.9;
    float3 fireColour = colour;

    float3 black = float3(0, 0, 0);
    float whiteness = (uv.y) * samp.r;

    // fade edge from white to out
    //float whiteInterpolant = (1 - uv.y) / (1 - whiteThreshold);
    //opacity = uv.x - fadeOutProgress;

    float3 final = lerp(fireColour + float3(whiteness, whiteness, whiteness), black, samp.r < 0.5);
    opacity = uv.x; // * (1 - easeInExpo(fadeOutProgress));
    
    return float4(final, 1) * opacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}