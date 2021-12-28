#version 400 core
#define MAX_LIGHTS 8
#define MAX_JOINTS 50

in vec3 position;
in vec2 textureCoords;
in vec3 normal;
in vec3 tangent;
in ivec3 jointIndices;
in vec3 weights;

out vec2 pass_textureCoords;
out vec3 surfaceNormal;
out vec3 toLightVector[MAX_LIGHTS];
out vec3 toCameraVector;
out float visibility;
out vec4 shadowCoords;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 lightPosition[MAX_LIGHTS];
uniform vec2 textureOffset;

uniform float useFakeLighting;
uniform float density;
uniform float gradient;

uniform int numberOfRows;

uniform int lightCount;

uniform vec4 clipPlane;

uniform mat4 toShadowMapSpace;
uniform float shadowDistance;

uniform mat4 jointTransforms[MAX_JOINTS];
uniform int jointTransformCount;

const float transitionDistance = 10.0;

void main(void)
{
	vec3 actualNormal = normal;
	if (useFakeLighting > 0.5)
	{
		actualNormal = vec3(0.0, 1.0, 0.0);
	}

	vec4 totalPosition = vec4(0.0);
	vec4 totalNormal = vec4(0.0);
	if (jointTransformCount == 0) {
		totalPosition = vec4(position, 1.0);
		totalNormal = vec4(actualNormal, 0.0);
	}
	else {
		for(int i = 0; i < jointTransformCount; i++){
			mat4 jointTransform = jointTransforms[jointIndices[i]];
			vec4 posePosition = jointTransform * vec4(position, 1.0);
			totalPosition += posePosition * weights[i];
		
			vec4 worldNormal = jointTransform * vec4(actualNormal, 0.0);
			totalNormal += worldNormal * weights[i];
		}
	}

	vec4 worldPosition = transformationMatrix * totalPosition;
	vec4 positionRelativeToCam = viewMatrix * worldPosition;

	surfaceNormal = (transformationMatrix * totalNormal).xyz;
	
	pass_textureCoords = (textureCoords / numberOfRows) + textureOffset;

	gl_ClipDistance[0] = dot(worldPosition, clipPlane);
	gl_Position =  projectionMatrix * positionRelativeToCam;

	for (int i = 0; i < lightCount; i++)
	{
		toLightVector[i] = lightPosition[i] - worldPosition.xyz;
	}

	toCameraVector = (inverse(viewMatrix) * vec4(0.0, 0.0, 0.0, 1.0)).xyz - worldPosition.xyz;

	float distance = length(positionRelativeToCam.xyz);
	visibility = exp(-pow((distance * density), gradient));
	visibility = clamp(visibility, 0.0, 1.0);
	
	shadowCoords = toShadowMapSpace * worldPosition;
	distance = distance - (shadowDistance - transitionDistance);
	distance = distance / transitionDistance;
	shadowCoords.w = clamp(1.0 - distance, 0.0, 1.0);
}