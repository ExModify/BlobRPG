#version 330
#define MAX_LIGHTS 8

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector[MAX_LIGHTS];
in vec3 toCameraVector;
in vec4 shadowCoords;

in float visibility;

uniform sampler2D textureSampler;

uniform sampler2D specularMap;
uniform float useSpecularMap;

uniform vec3 lightColor[MAX_LIGHTS];
uniform vec3 lightAttenuation[MAX_LIGHTS];
uniform float reflectivity;
uniform float shineDamper;
uniform vec3 fogColor;

uniform int lightCount;

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

void main(void) {
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


	vec3 unitNormal = NormalizeIfGreaterThanZero(surfaceNormal);
	vec3 unitVectorToCamera = NormalizeIfGreaterThanZero(toCameraVector);

	vec3 totalDiffuse = vec3(0.0);
	vec3 totalSpecular = vec3(0.0);

	for (int i = 0; i < lightCount; i++)
	{
		float distance = length(toLightVector[i]);
		float attFactor = lightAttenuation[i].x + (lightAttenuation[i].y * distance) + (lightAttenuation[i].z * distance * distance);
		vec3 unitLight = NormalizeIfGreaterThanZero(toLightVector[i]);

		float nDotl = dot(unitNormal, unitLight);
		float brightness = max(nDotl, 0.0);


		vec3 lightDirection = -unitLight;
		vec3 reflectedLightDirection = reflect(lightDirection, unitNormal);

		float specularFactor = dot(reflectedLightDirection, unitVectorToCamera);
		specularFactor = max(specularFactor, 0.0);

		float dampedFactor = pow(specularFactor, shineDamper);
		
		totalDiffuse = totalDiffuse + (brightness * lightColor[i]) / attFactor;
		totalSpecular = totalSpecular + (dampedFactor * reflectivity * lightColor[i]) / attFactor;
	}
	totalDiffuse = max(totalDiffuse * lightFactor, 0.1);

	vec4 textureColor = texture(textureSampler, pass_textureCoords);

	if (textureColor.a < 0.5)
	{
		discard;
	}
	
	out_BrightColor = vec4(0.0);
	if (useSpecularMap > 0.5) {
		vec4 mapInfo = texture(specularMap, pass_textureCoords);
		totalSpecular *= mapInfo.r;

		if (mapInfo.g > 0.2) {
			out_BrightColor = textureColor + vec4(totalSpecular, 1.0);
			totalDiffuse = vec3(mapInfo.g);
		}
	}


	out_Color = vec4(totalDiffuse, 1.0) * textureColor + vec4(totalSpecular, 1.0);
	out_Color = mix(vec4(fogColor, 1.0), out_Color, visibility);
}