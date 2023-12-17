sampler mainTexture : register(s0);

texture noiseMap;
sampler noiseMapSampler : register(s1);

matrix worldViewProjection;

float uTime;
int duration;
int direction; // 1 or -1
float3 colour;
//float timeToStartFadingOut = duration / 10f * 9f;

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
    float2 uv = input.TextureCoordinates;

    float2 centre = float2(0.5, 0.5); // define the centre of the shader
    
    float distanceFromCentre = sqrt(pow(uv.x - centre.x, 2.) + pow(uv.y - centre.y, 2.)); // calculate how far we are from the centre
    
    float maxDistanceFromCentre = length(centre);
    
    distanceFromCentre = distanceFromCentre / maxDistanceFromCentre;
    
    float colourAmount;
    
    if (distanceFromCentre != 0.)
    {
        // decreasing the subtract number makes outline thicker
        colourAmount = pow(2., 10. * distanceFromCentre - 9.);
    }
    
    float3 col = colourAmount * colour * tex2D(mainTexture, uv).r;

    // Output to screen
    return float4(col, 0);
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
