#version 400

in vec3 textureCoords;

uniform samplerCube cubeMap;
uniform samplerCube cubeMap2;

uniform float blendFactor;

uniform vec3 fogColor;

const float lowerLimit = 0.0;
const float upperLimit = 30.0;

layout (location = 0) out vec4 out_Color;
layout (location = 1) out vec4 out_BrightColor;

void main(void) {

	vec4 texture1 = texture(cubeMap, textureCoords);
	vec4 texture2 = texture(cubeMap2, textureCoords);
	
	vec4 finalColor = mix(texture1, texture2, blendFactor);

	float factor = (textureCoords.y - lowerLimit) / (upperLimit - lowerLimit);
	factor = clamp(factor, 0.0, 1.0);

	out_Color = mix(vec4(fogColor, 1.0), finalColor, factor);
	out_BrightColor = vec4(0.0);
}