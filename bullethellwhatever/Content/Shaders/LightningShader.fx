sampler mainTexture : register(s0);
//sampler2D noise : register(s1);
texture noiseMap;

sampler2D noiseSampler = sampler_state
{
    Texture = <noiseMap>;
};

matrix worldViewProjection;

float uTime;
float3 colour;
int direction; // 1 or -1

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

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, worldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //mainTexture needs to be used to occupy register 0
    
    //float2 dummy = tex2D(mainTexture, 0.3) * 0.001f;
    
    float2 uv = input.TextureCoordinates;
    
    float distanceFromCenterX = abs(0.5 - uv.x);
    float whiteAmount = 0.8f - pow(distanceFromCenterX, 2.0);
    float3 white = float3(whiteAmount, whiteAmount, whiteAmount);
    float opacity = pow(1 - distanceFromCenterX, 5);
    return float4(colour + white, 1) * opacity + 0.0f * tex2D(noiseSampler, uv);
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
