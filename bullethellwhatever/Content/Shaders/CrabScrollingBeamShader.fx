texture noiseMap;

sampler2D noiseSampler = sampler_state
{
    Texture = <noiseMap>;
};

sampler2D mainTexture;

matrix worldViewProjection;

float deathrayPulsationRate;
float uTime;
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
    
    float2 dummy = tex2D(mainTexture, 0.3) * 0.001f;
    
    float2 uv = input.TextureCoordinates + dummy;
    
    deathrayPulsationRate = 10; // decrease to pulse faster
    float pulseThickness = 0.5;
    
    float sineOscillation = pulseThickness * sin(uTime / deathrayPulsationRate - 12 * uv.y);
    float distanceFromCentre = abs(uv.x - 0.5);
    
        // Calculate the opacity of the point using an exponential function to make the opacity decrease drastically.
        // A sine is used to vary the exponent to produce a pulsing effect.
    
    float opacity = pow(1 - distanceFromCentre, sineOscillation + 7);
    
        // Adjust red and white values to achieve the desired effect.
    
        //float red =  7. * pow(4., uv.x);
        //float white = 1. - pow(5. * sin(uv.x) + 15., abs(uv.x - 0.5));
        
    float red = 2.5 - distanceFromCentre;
    float white = 0.15 * sineOscillation + 0.8 - distanceFromCentre * 2.;
    
    float scrollSpeed = -0.03;
        
    float2 noiseCoord = uv + float2(0, uTime * scrollSpeed);
    
    // use less noise at the ray's edge
    
    float4 final = float4(red, white, white, 1) + (1.2 - distanceFromCentre) * tex2D(noiseSampler, noiseCoord);
    
    return final * opacity;
    
    //return tex2D(noiseSampler, uv);
}

Technique Technique1
{
    pass ShaderPass
    {
        //VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
        
    }
}
