#version 400 core

in vec2 pass_textureCoords;
in vec3 surfaceNormal;
in vec3 toLightVector;
in vec3 toCameraVector;

in float visibility;

uniform sampler2D backgroundTexture;
uniform sampler2D rTexture;
uniform sampler2D gTexture;
uniform sampler2D bTexture;
uniform sampler2D blendTexture;

uniform vec3 lightColor;
uniform float reflectivity;
uniform float shineDamper;
uniform vec3 fogColor;

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
	vec4 blendMapColor = texture(blendTexture, pass_textureCoords);

	float backTextureAmount = 1 - (blendMapColor.r + blendMapColor.g + blendMapColor.b);
	vec2 tiledCoords = pass_textureCoords * 40.0;
	vec4 backgroundTextureColor = texture(backgroundTexture, tiledCoords) * backTextureAmount;
	vec4 rTextureColor = texture(rTexture, tiledCoords) * blendMapColor.r;
	vec4 gTextureColor = texture(gTexture, tiledCoords) * blendMapColor.g;
	vec4 bTextureColor = texture(bTexture, tiledCoords) * blendMapColor.b;

	vec4 totalColor = backgroundTextureColor + rTextureColor + gTextureColor + bTextureColor;

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

	out_Color = vec4(diffuse, 1.0) * totalColor + vec4(finalSpecular, 1.0);
	out_Color = mix(vec4(fogColor, 1.0), out_Color, visibility);
}