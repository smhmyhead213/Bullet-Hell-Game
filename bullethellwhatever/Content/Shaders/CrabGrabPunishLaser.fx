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
float oscillationVariance; // range from 0 - 0.5 ideally
float opacity;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};


struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    output.Position = mul(input.Position, view_projection);
    //output.Position = input.Position;
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
    float2 uv = input.TextureCoordinates;
    float4 baseColor = tex2D(TextureSampler, uv).rgba;
    float dummy = baseColor.r * 0.001;
    uv = input.TextureCoordinates + dummy;
    
    float centreX = 0.5;
    float halfWidth = 0.32;
    float4 backColour = float4(1, 0, 0, 1);
    float4 beamColour = float4(1, 1, 1, 1);
    
    float oscillationFactor = 1 + (sin(uTime / 10.0 + uv.y) * oscillationVariance);
    oscillationFactor = 1;
    
    float distFromCentre = abs(uv.x - centreX) * oscillationFactor; // trick it into thinking its further from the centre
    float opacityInterpolant = clamp(easeInExpo(2 * distFromCentre), 0.0, 1.0);
    float endOpacity = lerp(1, 0, opacityInterpolant) * opacity;
    float scrollOffset = (scrollSpeed * uTime) % 1 + dummy;
    float4 sample = NoiseTexture.Sample(TextureSampler2, uv + float2(scrollOffset, scrollOffset));
    float4 outColour = float4(0, 0, 0, 0);
    
    if (distFromCentre < halfWidth)
    {
        float interpolant = distFromCentre / halfWidth;
        outColour = lerp(beamColour, backColour, interpolant);
    }
    else
    {
        outColour = backColour;
    }
    
    return (outColour + sample) * endOpacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}