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
float3 colour;
float scrollSpeed;
float baseOpacity;

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

float easeInExpo(float x)
{
    return x == 0 ? 0 : pow(2, 10 * x - 10);
}

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
    //float2 dummy = NoiseTexture.Sample(TextureSampler, 0.3) * 0.001f;

    //float2 uv = input.TextureCoordinates + dummy;
    
    float2 uv = input.TextureCoordinates;
    float4 baseColor = tex2D(TextureSampler, uv).rgba;
    uv = uv + baseColor.r * 0.001f;
    
    float inCircleMultiplier = 1.0;
    
    float2 centre = float2(0.5, 0.5); // define the centre of the shader

    float maxDist = 0.5f; // dont display anything further than this from cnetre of texture
    float distanceFromCentre = sqrt(pow(uv.x - centre.x, 2.) + pow(uv.y - centre.y, 2.)); // calculate how far we are from the centre

    if (distanceFromCentre > maxDist)
    {
        inCircleMultiplier = 0;
    }
    
    float4 col = float4(colour, 0);
    float2 centreToUV = uv - centre;
    float mostOpaqueRadius = 0.3; // make it fade out slightly towards the edges by setting a max opacity slightly inwards
    float distFromMostOpaque = abs(distanceFromCentre - mostOpaqueRadius);
    float opacityInterpolant = 1.0 - (distFromMostOpaque / mostOpaqueRadius);
    float opacity = lerp(0.0, 1.0, easeInExpo(opacityInterpolant)); // 0 - 1 range
    float scrollOffset = (scrollSpeed * distanceFromCentre * uTime) % 1;
    float4 sample = NoiseTexture.Sample(NoiseSampler, centreToUV);
    

    return sample * baseOpacity * opacity * inCircleMultiplier;
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 MainVS();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}