﻿#version 400 core
#define MAX_LIGHTS 8

in vec3 position;
in vec2 textureCoords;
in vec3 normal;

out vec2 pass_textureCoords;
out vec3 surfaceNormal;
out vec3 toLightVector[MAX_LIGHTS];
out vec3 toCameraVector;
out float visibility;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 lightPositionEyeSpace[MAX_LIGHTS];
uniform vec2 textureOffset;

uniform float numberOfRows;

uniform float useFakeLighting;
uniform float density;
uniform float gradient;

uniform vec4 clipPlane;

uniform int lightCount;

void main(void){

	vec4 worldPosition = transformationMatrix * vec4(position,1.0);
	gl_ClipDistance[0] = dot(worldPosition, clipPlane);
	mat4 modelViewMatrix = viewMatrix * transformationMatrix;
	vec4 positionRelativeToCam = modelViewMatrix * vec4(position,1.0);
	gl_Position = projectionMatrix * positionRelativeToCam;
	
	pass_textureCoords = (textureCoords / numberOfRows) + textureOffset;
	
	vec3 actualNormal = normal;
	if (useFakeLighting > 0.5)
	{
		actualNormal = vec3(0.0, 1.0, 0.0);
	}

	surfaceNormal = (modelViewMatrix * vec4(actualNormal, 0.0)).xyz;
	for(int i = 0; i < lightCount; i++){
		toLightVector[i] = lightPositionEyeSpace[i] - positionRelativeToCam.xyz;
	}
	toCameraVector = -positionRelativeToCam.xyz;
	
	float distance = length(positionRelativeToCam.xyz);
	visibility = exp(-pow((distance * density),gradient));
	visibility = clamp(visibility, 0.0, 1.0);
}