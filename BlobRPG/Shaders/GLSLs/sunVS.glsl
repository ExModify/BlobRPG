#version 400

in vec2 position;
out vec2 textureCoords;

uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

uniform vec4 clipPlane;

void main(void) {
	
	vec4 worldPosition = vec4(position, 0.0, 1.0);

	gl_Position = projectionMatrix * viewMatrix * worldPosition;
	gl_ClipDistance[0] = dot(worldPosition, clipPlane);

	textureCoords = position + vec2(0.5, 0.5);
	textureCoords.y = 1.0 - textureCoords.y;

}