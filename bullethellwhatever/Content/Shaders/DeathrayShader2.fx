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
    float2 uv = input.TextureCoordinates;
    float4 baseColor = tex2D(TextureSampler, uv).rgba;
    float dummy = baseColor.r * 0.001;
    uv = input.TextureCoordinates + dummy;
    
    float deathrayPulsationRate = 1.0;
    float sineOscillation = sin(uTime / deathrayPulsationRate - 12 * uv.y);
    float distanceFromCentre = abs(uv.x - 0.5);
    
    // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
    // A sine is used to vary the exponent to produce a pulsing effect.

    float opacity = pow(1 - distanceFromCentre, sineOscillation);
    
    // Adjust red and white values to achieve the desired effect.
    
        //float red =  7. * pow(4., uv.x);
        //float white = 1. - pow(5. * sin(uv.x) + 15., abs(uv.x - 0.5));
        
    float colourAmount = 2.5 - distanceFromCentre;
    float white = 0.15 * sineOscillation + 0.8 - distanceFromCentre * 2.;

    float2 noiseCoord = uv + float2(0, uTime * scrollSpeed);
    float scrollOffset = (scrollSpeed * uTime) % 1 + dummy;
    float4 sample = NoiseTexture.Sample(TextureSampler2, uv + float2(0, scrollOffset));
    float4 final = float4(lerp(white, colourAmount, colour.r), lerp(white, colourAmount, colour.g), lerp(white, colourAmount, colour.b), 1) + sample;
    return final * opacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}