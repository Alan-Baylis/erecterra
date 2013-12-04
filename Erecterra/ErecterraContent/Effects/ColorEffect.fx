// Matrices
float4x4 World;
float4x4 View;
float4x4 Projection;

// Ambient
float4 AmbientColor = float4(1, 1, 1, 1); // color of ambient light
float AmbientIntensity = 0.8; // intensity of the light

// Sun
float4 SunDiffuseColor; // color from the sun
float SunDiffuseIntensity; // intensity of the sun
float3 SunDiffuseLightDirection; // direction of the sun

// Moon
float4 MoonDiffuseColor; // color from the Moon
float MoonDiffuseIntensity; // intensity of the Moon
float3 MoonDiffuseLightDirection; // direction of the Moon

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Normal : TEXCOORD0;
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Color = input.Color;
	output.Normal = normalize(mul(input.Normal, World));

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float3 sunNormLightDirection = normalize( SunDiffuseLightDirection );
	float4 sunDiffuse = dot( sunNormLightDirection, input.Normal) * SunDiffuseIntensity * SunDiffuseColor;
	float3 moonNormLightDirection = normalize( MoonDiffuseLightDirection );
	float4 moonDiffuse = dot( moonNormLightDirection, input.Normal) * MoonDiffuseIntensity * MoonDiffuseColor;
	float4 ambient = AmbientIntensity * AmbientColor;

    return input.Color * saturate(sunDiffuse + moonDiffuse + ambient);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
