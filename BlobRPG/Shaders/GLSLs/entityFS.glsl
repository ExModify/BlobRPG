﻿#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;

uniform sampler2D textureSampler;

uniform vec3 lightColor;

out vec4 out_Color;

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
	vec3 unitNormal = NormalizeIfGreaterThanZero(surfaceNormal);
	vec3 unitLight = NormalizeIfGreaterThanZero(toLightVector);

	float nDotl = dot(unitNormal, unitLight);
	float brightness = max(nDotl, 0.0);

	vec3 diffuse = brightness * lightColor;

	out_Color = vec4(diffuse, 1.0) * texture(textureSampler, pass_textureCoords);
	
}