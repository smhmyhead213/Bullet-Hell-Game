sampler mainTexture : register(s0);

texture noiseMap;

sampler2D noiseMapSampler = sampler_state // test in future if this works
{
    Texture = <noiseMap>;
};

matrix WorldViewProjection;
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
    float4 pos = mul(input.Position, WorldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 dummy = tex2D(mainTexture, 0.3) * 0.001f;
    float2 uv = input.TextureCoordinates + dummy;
    
    float distanceFromCenter = abs(0.5 - uv.x);
    float opacity = 1 - 2 * distanceFromCenter;
    // amplify already bright areas and diminish everywhere else
    float scrollOffset = (scrollSpeed * uTime) % 1;
    float4 sample = tex2D(noiseMapSampler, uv + float2(scrollOffset, scrollOffset));
    // controls the threshold above which to be bright
    float lenience = 0.9;
    float strength = pow(sample + lenience, 5) - 0.2;
    //return sample;
    return float4(colour, 1) * strength * opacity;
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 MainPS();
        
    }
}