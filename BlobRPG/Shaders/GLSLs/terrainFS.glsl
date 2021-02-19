#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;

uniform sampler2D textureSampler;

uniform vec3 lightColor;
uniform float reflectivity;
uniform float shineDamper;

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
	float brightness = max(nDotl, 0.1);

	vec3 diffuse = brightness * lightColor;

	vec3 unitVectorToCamera = NormalizeIfGreaterThanZero(toCameraVector);
	vec3 lightDirection = -unitLight;
	vec3 reflectedLightDirection = reflect(lightDirection, unitNormal);

	float specularFactor = dot(reflectedLightDirection, unitVectorToCamera);
	specularFactor = max(specularFactor, 0.0);

	float dampedFactor = pow(specularFactor, shineDamper);

	vec3 finalSpecular = dampedFactor * reflectivity * lightColor;

	out_Color = vec4(diffuse, 1.0) * texture(textureSampler, pass_textureCoords) + vec4(finalSpecular, 1.0);
	
}