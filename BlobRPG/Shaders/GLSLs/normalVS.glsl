#version 400 core
#define MAX_LIGHTS 8

in vec3 position;
in vec2 textureCoords;
in vec3 normal;
in vec3 tangent;

out vec2 pass_textureCoords;
out vec3 toLightVector[MAX_LIGHTS];
out vec3 toCameraVector;
out float visibility;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;
uniform vec3 lightPositionEyeSpace[MAX_LIGHTS];
uniform vec2 textureOffset;

uniform int numberOfRows;

uniform float useFakeLighting;
uniform float density;
uniform float gradient;

uniform vec4 clipPlane;

uniform int lightCount;


uniform mat4 toShadowMapSpace;
uniform float shadowDistance;
const float transitionDistance = 10.0;
out vec4 shadowCoords;

vec3 NormalizeIfGreaterThanZero(vec3 result)
{
	if(result.x != 0 || result.y != 0 || result.z != 0)
	{
		result = normalize(result);
	}
	return result;
}

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

	vec3 surfaceNormal = (modelViewMatrix * vec4(actualNormal, 0.0)).xyz;

	vec3 norm = NormalizeIfGreaterThanZero(surfaceNormal);
	vec3 tang = NormalizeIfGreaterThanZero((modelViewMatrix * vec4(tangent, 0.0)).xyz);
	vec3 bitang = NormalizeIfGreaterThanZero(cross(norm, tang));
	mat3 toTangentSpace = mat3(
		tang.x, bitang.x, norm.x,
		tang.y, bitang.y, norm.y,
		tang.z, bitang.z, norm.z
	);

	for(int i = 0; i < lightCount; i++){
		toLightVector[i] = toTangentSpace * (lightPositionEyeSpace[i] - positionRelativeToCam.xyz);
	}
	toCameraVector = toTangentSpace * (-positionRelativeToCam.xyz);
	
	float distance = length(positionRelativeToCam.xyz);
	visibility = exp(-pow((distance * density),gradient));
	visibility = clamp(visibility, 0.0, 1.0);
	
	shadowCoords = toShadowMapSpace * worldPosition;
	distance = distance - (shadowDistance - transitionDistance);
	distance = distance / transitionDistance;
	shadowCoords.w = clamp(1.0 - distance, 0.0, 1.0);
}