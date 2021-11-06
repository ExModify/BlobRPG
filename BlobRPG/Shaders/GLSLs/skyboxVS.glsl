#version 400

in vec3 position;
out vec3 textureCoords;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

uniform vec4 clipPlane;

void main(void) {
	
	vec4 worldPosition = vec4(position, 1.0);

	gl_Position = projectionMatrix * viewMatrix * worldPosition;
	gl_ClipDistance[0] = dot(worldPosition, clipPlane);

	textureCoords = position;

}