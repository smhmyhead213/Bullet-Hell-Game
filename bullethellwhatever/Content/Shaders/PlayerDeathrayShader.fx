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
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{       
    // sample to avoid compiling out
    float2 uv = input.TextureCoordinates;
    float4 baseColor = tex2D(TextureSampler, uv).rgba;
    float dummy = baseColor.r * 0.001;

    float scrollOffset = (scrollSpeed * uTime) % 1 + dummy;
    float4 samp = NoiseTexture.Sample(TextureSampler2, uv + float2(scrollOffset, scrollOffset));
    
    return samp;
    
    //float distanceFromCenter = abs(0.5 - uv.x) + dummy;
    
    //float opacity = 1 - 2 * distanceFromCenter;
    //// amplify already bright areas and diminish everywhere else
    //float scrollOffset = (scrollSpeed * uTime) % 1;
    //float4 sample = NoiseTexture.Sample(TextureSampler2, uv + float2(scrollOffset, scrollOffset));
    //// controls the threshold above which to be bright
    //float lenience = 0.9;
    //float strength = pow(sample + lenience, 5) - 0.2;
    //return sample;
    //return float4(colour, 1) * strength * opacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}