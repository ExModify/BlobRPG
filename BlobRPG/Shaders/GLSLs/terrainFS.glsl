#version 400 core
#define MAX_LIGHTS 8

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector[MAX_LIGHTS];
in vec3 toCameraVector;

in float visibility;

uniform sampler2D backgroundTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;
uniform sampler2D blendTexture;

uniform vec3 lightColor[MAX_LIGHTS];
uniform vec3 lightAttenuation[MAX_LIGHTS];
uniform float reflectivity;
uniform float shineDamper;
uniform vec3 fogColor;

uniform int lightCount;


in vec4 shadowCoords;
uniform sampler2D shadowMap;
uniform float shadowMapSize;
uniform int pcfCount;

layout (location = 0) out vec4 out_Color;
layout (location = 1) out vec4 out_BrightColor;

vec3 NormalizeIfGreaterThanZero(vec3 result)
{
	if(result.x != 0 || result.y != 0 || result.z != 0)
	{
		result = normalize(result);
	}
	return result;
}

void main(void)
{
	float totalTexels = (pcfCount * 2.0 + 1.0);
	totalTexels = totalTexels * totalTexels;
	float texelSize = 1.0 / shadowMapSize;
	float total = 0.0;

	for (int x = -pcfCount; x <= pcfCount; x++) {
		for (int y = -pcfCount; y <= pcfCount; y++) {
			float objectNearestLight = texture(shadowMap, shadowCoords.xy + vec2(x, y) * texelSize).r;
			if (shadowCoords.z > objectNearestLight + 0.002) {
				total += 1.0;
			}
		}
	}
	total /= totalTexels;

	float lightFactor = 1.0 - (total * shadowCoords.w);

	vec4 blendMapColor = texture(blendTexture, pass_textureCoords);

	float backTextureAmount = 1 - (blendMapColor.r + blendMapColor.g + blendMapColor.b);
	vec2 tiledCoords = pass_textureCoords * 40.0;
	vec4 backgroundTextureColor = texture(backgroundTexture, tiledCoords) * backTextureAmount;
	vec4 rTextureColor = texture(rTexture, tiledCoords) * blendMapColor.r;
	vec4 gTextureColor = texture(gTexture, tiledCoords) * blendMapColor.g;
	vec4 bTextureColor = texture(bTexture, tiledCoords) * blendMapColor.b;

	vec4 totalColor = backgroundTextureColor + rTextureColor + gTextureColor + bTextureColor;

	vec3 unitNormal = NormalizeIfGreaterThanZero(surfaceNormal);

	
	vec3 totalDiffuse = vec3(0.0);
	vec3 totalSpecular = vec3(0.0);

	for (int i = 0; i < lightCount; i++)
	{
		float distance = length(toLightVector[i]);
		float attFactor = lightAttenuation[i].x + (lightAttenuation[i].y * distance) + (lightAttenuation[i].z * distance * distance);
		vec3 unitLight = NormalizeIfGreaterThanZero(toLightVector[i]);

		float nDotl = dot(unitNormal, unitLight);
		float brightness = max(nDotl, 0.0);


		vec3 unitVectorToCamera = NormalizeIfGreaterThanZero(toCameraVector);
		vec3 lightDirection = -unitLight;
		vec3 reflectedLightDirection = reflect(lightDirection, unitNormal);

		float specularFactor = dot(reflectedLightDirection, unitVectorToCamera);
		specularFactor = max(specularFactor, 0.0);

		float dampedFactor = pow(specularFactor, shineDamper);
	
		totalDiffuse = totalDiffuse + (brightness * lightColor[i]) / attFactor;
		totalSpecular = totalSpecular + (dampedFactor * reflectivity * lightColor[i]) / attFactor;
	}
	totalDiffuse = max(totalDiffuse * lightFactor, 0.1);

	out_Color = vec4(totalDiffuse, 1.0) * totalColor + vec4(totalSpecular, 1.0);
	out_Color = mix(vec4(fogColor, 1.0), out_Color, visibility);
	out_BrightColor = vec4(0.0);
}